using System.Security.Claims;
using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.SiteSettings;
using Service.Helpers.DTOs.UserSettings;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IUserSettingsService _userSettingsService;
        private readonly ISiteSettingsService _siteSettingsService;
        private readonly IAppUserService _userService;

        public SettingsController(
            IUserSettingsService userSettingsService,
            ISiteSettingsService siteSettingsService,
            IAppUserService userService)
        {
            _userSettingsService = userSettingsService;
            _siteSettingsService = siteSettingsService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var settings = await EnsureUserSettingsAsync(currentUser.Id);
            return Ok(ToDto(settings));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMine([FromBody] UserSettingsDto request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            if (!TryNormalizePrivacy(request.DefaultPrivacy, out var privacy))
                return BadRequest(new { message = "Choose public, friends, or communities." });

            if (!TryNormalizeTheme(request.Theme, out var theme))
                return BadRequest(new { message = "Choose system, light, or dark." });

            var blocked = await NormalizeBlockedIdsAsync(request.BlockedUserIds, currentUser);

            var settings = await EnsureUserSettingsAsync(currentUser.Id);
            settings.DefaultPrivacy = privacy;
            settings.AllowRequests = request.AllowRequests;
            settings.ShowSuggestions = request.ShowSuggestions;
            settings.AllowDms = request.AllowDms;
            settings.NotifyLikes = request.NotifyLikes;
            settings.NotifyReplies = request.NotifyReplies;
            settings.NotifyReposts = request.NotifyReposts;
            settings.NotifyRequests = request.NotifyRequests;
            settings.NotifyCommunities = request.NotifyCommunities;
            settings.Theme = theme;
            settings.ReduceMotion = request.ReduceMotion;
            settings.Autoplay = request.Autoplay;
            settings.BlockedUserPublicIds = JsonSerializer.Serialize(blocked);
            await _userSettingsService.EditAsync(settings);
            return Ok(ToDto(settings));
        }

        [HttpPost("blocked/{userId}")]
        public async Task<IActionResult> Block(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var target = await _userService.GetByPublicIdAsync(userId);
            if (target is null || target.IsDeleted)
                return NotFound();

            if (target.Id == currentUser.Id)
                return BadRequest(new { message = "You cannot block yourself." });

            var settings = await EnsureUserSettingsAsync(currentUser.Id);
            var blocked = ReadBlockedIds(settings.BlockedUserPublicIds);
            if (!blocked.Contains(target.PublicId, StringComparer.OrdinalIgnoreCase))
            {
                blocked.Add(target.PublicId);
                settings.BlockedUserPublicIds = JsonSerializer.Serialize(blocked);
                await _userSettingsService.EditAsync(settings);
            }

            return Ok(ToDto(settings));
        }

        [HttpDelete("blocked/{userId}")]
        public async Task<IActionResult> Unblock(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var settings = await EnsureUserSettingsAsync(currentUser.Id);
            var blocked = ReadBlockedIds(settings.BlockedUserPublicIds);
            var removed = blocked.RemoveAll(id => string.Equals(id, userId, StringComparison.OrdinalIgnoreCase));
            if (removed > 0)
            {
                settings.BlockedUserPublicIds = JsonSerializer.Serialize(blocked);
                await _userSettingsService.EditAsync(settings);
            }

            return Ok(ToDto(settings));
        }

        [HttpGet("site")]
        public async Task<IActionResult> GetSite()
        {
            var settings = await EnsureSiteSettingsAsync();
            return Ok(ToDto(settings));
        }

        [HttpPut("site")]
        public async Task<IActionResult> UpdateSite([FromBody] UpdateSiteSettingsRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            if (currentUser.Role is not (UserRole.Admin or UserRole.SuperAdmin))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only an admin can change site settings." });

            var settings = await EnsureSiteSettingsAsync();
            if (!TryApply(settings, request, out var error))
                return BadRequest(new { message = error });

            await _siteSettingsService.EditAsync(settings);
            return Ok(ToDto(settings));
        }

        private async Task<UserSettings> EnsureUserSettingsAsync(int userId)
        {
            var settings = await _userSettingsService.GetByUserIdAsync(userId);
            if (settings is not null)
            {
                if (settings.IsDeleted)
                {
                    settings.IsDeleted = false;
                    await _userSettingsService.EditAsync(settings);
                }

                return settings;
            }

            settings = new UserSettings
            {
                UserId = userId,
                BlockedUserPublicIds = "[]"
            };

            try
            {
                await _userSettingsService.CreateAsync(settings);
            }
            catch (DbUpdateException)
            {
                var existing = await _userSettingsService.GetByUserIdAsync(userId);
                if (existing is null)
                    throw;

                settings = existing;
            }

            return settings;
        }

        private async Task<SiteSettings> EnsureSiteSettingsAsync()
        {
            var settings = await _siteSettingsService.GetCurrentAsync();
            if (settings is not null)
            {
                if (settings.IsDeleted)
                {
                    settings.IsDeleted = false;
                    await _siteSettingsService.EditAsync(settings);
                }

                return settings;
            }

            settings = new SiteSettings();
            try
            {
                await _siteSettingsService.CreateAsync(settings);
            }
            catch (DbUpdateException)
            {
                var existing = await _siteSettingsService.GetCurrentAsync();
                if (existing is null)
                    throw;

                settings = existing;
            }

            return settings;
        }

        private async Task<List<string>> NormalizeBlockedIdsAsync(IEnumerable<string>? ids, AppUser currentUser)
        {
            var blocked = new List<string>();
            foreach (var value in ids ?? [])
            {
                var publicId = value?.Trim();
                if (string.IsNullOrWhiteSpace(publicId))
                    continue;

                if (string.Equals(publicId, currentUser.PublicId, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (blocked.Contains(publicId, StringComparer.OrdinalIgnoreCase))
                    continue;

                var user = await _userService.GetByPublicIdAsync(publicId);
                if (user is null || user.IsDeleted)
                    continue;

                blocked.Add(user.PublicId);
            }

            return blocked;
        }

        private static string FirstMark(string siteName)
        {
            var name = siteName.Trim();
            return name.Length == 0 ? "g" : name[..1];
        }

        private static List<string> ReadBlockedIds(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return [];

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json)?
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Select(id => id.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList() ?? [];
            }
            catch (JsonException)
            {
                return [];
            }
        }

        private static bool TryNormalizePrivacy(string? value, out string privacy)
        {
            privacy = value?.Trim().ToLowerInvariant() ?? string.Empty;
            return privacy is "public" or "friends" or "communities";
        }

        private static bool TryNormalizeTheme(string? value, out string theme)
        {
            theme = value?.Trim().ToLowerInvariant() ?? string.Empty;
            return theme is "system" or "light" or "dark";
        }

        private static bool TryApply(SiteSettings settings, UpdateSiteSettingsRequest request, out string error)
        {
            if (request.SiteName is not null)
            {
                var name = request.SiteName.Trim();
                if (name.Length == 0 || name.Length > 40)
                {
                    error = "Site name can be up to 40 characters.";
                    return false;
                }

                settings.SiteName = name;
            }

            if (request.Tagline is not null)
            {
                var tagline = request.Tagline.Trim();
                if (tagline.Length > 120)
                {
                    error = "Tagline can be up to 120 characters.";
                    return false;
                }

                settings.Tagline = tagline;
            }

            if (request.LogoMark is not null)
            {
                var mark = request.LogoMark.Trim();
                if (mark.Length > 2)
                {
                    error = "Logo letter can be up to 2 characters.";
                    return false;
                }

                settings.LogoMark = mark.Length == 0 ? FirstMark(settings.SiteName) : mark;
            }

            if (request.FooterNote is not null)
            {
                var footer = request.FooterNote.Trim();
                if (footer.Length > 160)
                {
                    error = "Footer note can be up to 160 characters.";
                    return false;
                }

                settings.FooterNote = footer;
            }

            if (request.LogoUrl is not null)
                settings.LogoUrl = request.LogoUrl.Trim();

            if (!TryApplyBody(request.AboutBody, value => settings.AboutBody = value, "About", out error))
                return false;

            if (!TryApplyBody(request.GuidelinesBody, value => settings.GuidelinesBody = value, "Guidelines", out error))
                return false;

            if (!TryApplyBody(request.PrivacyBody, value => settings.PrivacyBody = value, "Privacy", out error))
                return false;

            error = string.Empty;
            return true;
        }

        private static bool TryApplyBody(string? value, Action<string> assign, string label, out string error)
        {
            if (value is null)
            {
                error = string.Empty;
                return true;
            }

            var body = value.Trim();
            if (body.Length > 2000)
            {
                error = $"{label} can be up to 2000 characters.";
                return false;
            }

            assign(body);
            error = string.Empty;
            return true;
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

        private static UserSettingsDto ToDto(UserSettings settings)
        {
            return new UserSettingsDto
            {
                DefaultPrivacy = settings.DefaultPrivacy,
                AllowRequests = settings.AllowRequests,
                ShowSuggestions = settings.ShowSuggestions,
                AllowDms = settings.AllowDms,
                NotifyLikes = settings.NotifyLikes,
                NotifyReplies = settings.NotifyReplies,
                NotifyReposts = settings.NotifyReposts,
                NotifyRequests = settings.NotifyRequests,
                NotifyCommunities = settings.NotifyCommunities,
                Theme = settings.Theme,
                ReduceMotion = settings.ReduceMotion,
                Autoplay = settings.Autoplay,
                BlockedUserIds = ReadBlockedIds(settings.BlockedUserPublicIds)
            };
        }

        private static SiteSettingsDto ToDto(SiteSettings settings)
        {
            return new SiteSettingsDto
            {
                SiteName = settings.SiteName,
                Tagline = settings.Tagline,
                LogoMark = settings.LogoMark,
                LogoUrl = settings.LogoUrl,
                FooterNote = settings.FooterNote,
                AboutBody = settings.AboutBody,
                GuidelinesBody = settings.GuidelinesBody,
                PrivacyBody = settings.PrivacyBody
            };
        }
    }
}
