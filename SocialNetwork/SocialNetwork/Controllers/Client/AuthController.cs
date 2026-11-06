using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Helpers.DTOs.AppUser;
using Service.Helpers.Validators;
using Service.Services.Interfaces;
using SocialNetwork.Email;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const int PasswordIterations = 100_000;

        private readonly IAppUserService _userService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<UpdateProfileRequest> _profileValidator;
        private readonly IValidator<ForgotPasswordRequest> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAppUserService userService,
            IUserSettingsService userSettingsService,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator,
            IValidator<UpdateProfileRequest> profileValidator,
            IValidator<ForgotPasswordRequest> forgotPasswordValidator,
            IValidator<ResetPasswordRequest> resetPasswordValidator,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _userService = userService;
            _userSettingsService = userSettingsService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _profileValidator = profileValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var error = await _registerValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var name = request.FullName.Trim();
            var username = NormalizeUsername(request.Username);
            var email = NormalizeEmail(request.Email);

            if (await _userService.UsernameTakenAsync(username))
                return BadRequest(new { message = "That username is already taken." });

            if (await _userService.EmailTakenAsync(email))
                return BadRequest(new { message = "That email is already in use." });

            var user = new AppUser
            {
                PublicId = Guid.NewGuid().ToString(),
                Name = name,
                Username = username,
                Email = email,
                PasswordHash = HashPassword(request.Password),
                Bio = request.Bio?.Trim() ?? string.Empty,
                Location = request.Location?.Trim() ?? string.Empty,
                PhotoUrl = request.Photo?.Trim() ?? string.Empty,
                Role = await _userService.CountAsync() == 0 ? UserRole.SuperAdmin : UserRole.Member,
                IsActive = true
            };

            try
            {
                await _userService.CreateAsync(user);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "That email or username is already in use." });
            }

            try
            {
                await _userSettingsService.CreateAsync(new UserSettings
                {
                    UserId = user.Id,
                    BlockedUserPublicIds = "[]"
                });
            }
            catch (DbUpdateException)
            {
            }

            return Ok(ToAuthResponse(user));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var error = await _loginValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var user = await _userService.FindByIdentityAsync(request.Identity);
            if (user is null || user.IsDeleted || !VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "That email or password is not right." });

            if (!user.IsActive)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "This account is suspended." });

            return Ok(ToAuthResponse(user));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var error = await _forgotPasswordValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var email = NormalizeEmail(request.Email);
            var user = await _userService.GetByEmailAsync(email);
            if (user is null)
                return BadRequest(new { message = "No account uses that email." });

            if (!user.IsActive)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "This account is suspended." });

            var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
            user.PasswordResetCodeHash = HashPassword(code);
            user.PasswordResetExpiresAt = DateTime.UtcNow.AddMinutes(15);
            await _userService.EditAsync(user);

            try
            {
                await _emailSender.SendAsync(
                    user.Email,
                    "Your Gather verification code",
                    $"Your verification code is {code}. It expires in 15 minutes.");
            }
            catch (Exception)
            {
                user.PasswordResetCodeHash = null;
                user.PasswordResetExpiresAt = null;
                await _userService.EditAsync(user);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "The verification email could not be sent." });
            }

            return Ok(new { message = "We sent a verification code to that email." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var error = await _resetPasswordValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var email = NormalizeEmail(request.Email);
            var user = await _userService.GetByEmailAsync(email);
            if (user is null || !user.IsActive)
                return BadRequest(new { message = "That verification code is not right or has expired." });

            var expired = user.PasswordResetExpiresAt is null || user.PasswordResetExpiresAt < DateTime.UtcNow;
            var codeMatches = !string.IsNullOrWhiteSpace(user.PasswordResetCodeHash)
                && VerifyPassword(request.Code.Trim(), user.PasswordResetCodeHash);
            if (expired || !codeMatches)
                return BadRequest(new { message = "That verification code is not right or has expired." });

            user.PasswordHash = HashPassword(request.Password);
            user.PasswordResetCodeHash = null;
            user.PasswordResetExpiresAt = null;
            await _userService.EditAsync(user);

            return Ok(new { message = "Your password has been updated." });
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await GetCurrentUserAsync();
            if (user is null)
                return Unauthorized();

            return Ok(ToUserDto(user));
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest request)
        {
            var user = await GetCurrentUserAsync();
            if (user is null)
                return Unauthorized();

            var error = await _profileValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            if (request.Name is not null)
                user.Name = request.Name.Trim();

            if (request.Username is not null)
            {
                var username = NormalizeUsername(request.Username);
                if (!string.Equals(username, user.Username, StringComparison.Ordinal) && await _userService.UsernameTakenAsync(username, user.Id))
                    return BadRequest(new { message = "That username is already taken." });

                user.Username = username;
            }

            if (request.Email is not null)
            {
                var email = NormalizeEmail(request.Email);
                if (!string.Equals(email, user.Email, StringComparison.OrdinalIgnoreCase) && await _userService.EmailTakenAsync(email, user.Id))
                    return BadRequest(new { message = "That email is already in use." });

                user.Email = email;
            }

            if (request.Bio is not null)
                user.Bio = request.Bio.Trim();

            if (request.Location is not null)
                user.Location = request.Location.Trim();

            if (request.Photo is not null)
                user.PhotoUrl = request.Photo.Trim();

            if (request.Banner is not null)
                user.BannerUrl = request.Banner.Trim();

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.PasswordHash = HashPassword(request.Password);

            try
            {
                await _userService.EditAsync(user);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "That email or username is already in use." });
            }

            return Ok(ToUserDto(user));
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var user = await GetCurrentUserAsync();
            if (user is null)
                return Unauthorized();

            await _userService.DeleteAsync(user);
            return NoContent();
        }

        private async Task<AppUser?> GetCurrentUserAsync()
        {
            var publicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(publicId))
                return null;

            var user = await _userService.GetByPublicIdAsync(publicId);
            if (user is null || user.IsDeleted || !user.IsActive)
                return null;

            return user;
        }

        private AuthResponse ToAuthResponse(AppUser user)
        {
            return new AuthResponse
            {
                Token = CreateToken(user),
                User = ToUserDto(user)
            };
        }

        private string CreateToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey()));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var minutes) ? minutes : 10080;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.PublicId),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, ToRole(user.Role))
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string SigningKey()
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
                throw new InvalidOperationException("Jwt:Key must be at least 32 bytes.");

            return key;
        }

        private static string NormalizeUsername(string username)
        {
            return username.Trim().TrimStart('@').ToLowerInvariant();
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, PasswordIterations, HashAlgorithmName.SHA256, 32);
            return $"pbkdf2${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split('$');
            if (parts.Length != 3 || parts[0] != "pbkdf2")
                return false;

            try
            {
                var salt = Convert.FromBase64String(parts[1]);
                var expected = Convert.FromBase64String(parts[2]);
                var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, PasswordIterations, HashAlgorithmName.SHA256, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static UserDto ToUserDto(AppUser user)
        {
            return new UserDto
            {
                Id = user.PublicId,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                Bio = user.Bio,
                Location = user.Location,
                Photo = user.PhotoUrl,
                Banner = user.BannerUrl,
                Tone = user.Tone,
                Role = ToRole(user.Role),
                Active = user.IsActive
            };
        }

        private static string ToRole(UserRole role)
        {
            return role switch
            {
                UserRole.SuperAdmin => "superadmin",
                UserRole.Admin => "admin",
                _ => "member"
            };
        }
    }
}
