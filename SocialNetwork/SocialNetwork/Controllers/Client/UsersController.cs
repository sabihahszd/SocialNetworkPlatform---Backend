using System.Security.Claims;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.DTOs.AppUser;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAppUserService _userService;

        public UsersController(IAppUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var users = await _userService.GetAllAsync();
            var response = users
                .Where(user => !user.IsDeleted)
                .OrderBy(user => user.Name)
                .Select(ToPublicUser)
                .ToList();

            return Ok(response);
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

        private static UserDto ToPublicUser(AppUser user)
        {
            return new UserDto
            {
                Id = user.PublicId,
                Name = user.Name,
                Username = user.Username,
                Bio = user.Bio,
                Location = user.Location,
                Photo = user.PhotoUrl,
                Banner = user.BannerUrl,
                Tone = user.Tone,
                Role = user.Role switch
                {
                    UserRole.SuperAdmin => "superadmin",
                    UserRole.Admin => "admin",
                    _ => "member"
                },
                Active = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
