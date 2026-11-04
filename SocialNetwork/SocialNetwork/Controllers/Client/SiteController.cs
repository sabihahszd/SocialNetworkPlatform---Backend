using System.Security.Claims;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.Common;
using Service.Helpers.DTOs.SiteSettings;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteController : ControllerBase
    {
        private readonly ISiteSettingsService _siteSettingsService;
        private readonly IAppUserService _userService;
        private readonly IPostService _postService;
        private readonly IReportService _reportService;
        private readonly ICommunityService _communityService;

        public SiteController(
            ISiteSettingsService siteSettingsService,
            IAppUserService userService,
            IPostService postService,
            IReportService reportService,
            ICommunityService communityService)
        {
            _siteSettingsService = siteSettingsService;
            _userService = userService;
            _postService = postService;
            _reportService = reportService;
            _communityService = communityService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var settings = await EnsureSiteSettingsAsync();
            return Ok(ToDto(settings));
        }

        [HttpGet("about")]
        public Task<IActionResult> About()
        {
            return Page("About", settings => settings.AboutBody);
        }

        [HttpGet("guidelines")]
        public Task<IActionResult> Guidelines()
        {
            return Page("Guidelines", settings => settings.GuidelinesBody);
        }

        [HttpGet("privacy")]
        public Task<IActionResult> Privacy()
        {
            return Page("Privacy", settings => settings.PrivacyBody);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> Stats()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            if (currentUser.Role is not (UserRole.Admin or UserRole.SuperAdmin))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only an admin can view site stats." });

            return Ok(new AdminStatsDto
            {
                Members = await _userService.CountAsync(),
                Posts = await _postService.CountAsync(),
                Reports = await _reportService.CountOpenAsync(),
                Circles = await _communityService.CountAsync()
            });
        }

        private async Task<IActionResult> Page(string title, Func<SiteSettings, string> body)
        {
            var settings = await EnsureSiteSettingsAsync();
            return Ok(new
            {
                title,
                siteName = settings.SiteName,
                body = body(settings)
            });
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
