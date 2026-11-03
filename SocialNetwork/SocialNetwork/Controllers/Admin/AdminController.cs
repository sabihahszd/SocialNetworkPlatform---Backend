using System.Security.Claims;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.AppUser;
using Service.Helpers.DTOs.Common;
using Service.Helpers.DTOs.Community;
using Service.Helpers.DTOs.Post;
using Service.Helpers.DTOs.Report;
using Service.Helpers.DTOs.SiteSettings;
using Service.Helpers.Validators;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class AdminController : ControllerBase
    {
        private readonly IAppUserService _userService;
        private readonly IPostService _postService;
        private readonly IReportService _reportService;
        private readonly ICommunityService _communityService;
        private readonly ISiteSettingsService _siteSettingsService;
        private readonly IValidator<UpdateUserRoleRequest> _roleValidator;
        private readonly IValidator<CreateCommunityRequest> _communityValidator;
        private readonly IValidator<UpdateSiteSettingsRequest> _siteValidator;

        public AdminController(
            IAppUserService userService,
            IPostService postService,
            IReportService reportService,
            ICommunityService communityService,
            ISiteSettingsService siteSettingsService,
            IValidator<UpdateUserRoleRequest> roleValidator,
            IValidator<CreateCommunityRequest> communityValidator,
            IValidator<UpdateSiteSettingsRequest> siteValidator)
        {
            _userService = userService;
            _postService = postService;
            _reportService = reportService;
            _communityService = communityService;
            _siteSettingsService = siteSettingsService;
            _roleValidator = roleValidator;
            _communityValidator = communityValidator;
            _siteValidator = siteValidator;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> Stats()
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var members = _userService.CountAsync();
            var posts = _postService.CountAsync();
            var reports = _reportService.CountOpenAsync();
            var circles = _communityService.CountAsync();
            await Task.WhenAll(members, posts, reports, circles);

            return Ok(new AdminStatsDto
            {
                Members = await members,
                Posts = await posts,
                Reports = await reports,
                Circles = await circles
            });
        }

        [HttpGet("members")]
        public async Task<IActionResult> GetMembers([FromQuery] string? q)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var users = await _userService.GetAllAsync();
            var term = q?.Trim();
            var response = users
                .Where(user => !user.IsDeleted)
                .Where(user => Matches(term, user.Name, user.Username))
                .OrderBy(user => user.Name)
                .Select(ToUserDto)
                .ToList();

            return Ok(response);
        }

        [HttpPut("members/{id}/role")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateUserRoleRequest request)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var currentUser = access.User!;
            if (currentUser.Role != UserRole.SuperAdmin)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only a superadmin can change roles." });

            var target = await _userService.GetByPublicIdAsync(id);
            if (target is null || target.IsDeleted)
                return NotFound();

            if (target.Role == UserRole.SuperAdmin || target.Id == currentUser.Id)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "This account's role cannot be changed here." });

            var error = await _roleValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var role = ParseRole(request.Role);

            target.Role = role;
            await _userService.EditAsync(target);
            return Ok(ToUserDto(target));
        }

        [HttpPost("members/{id}/status")]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var target = await _userService.GetByPublicIdAsync(id);
            if (target is null || target.IsDeleted)
                return NotFound();

            if (IsProtected(access.User!, target))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "This account cannot be suspended here." });

            target.IsActive = !target.IsActive;
            await _userService.EditAsync(target);
            return Ok(ToUserDto(target));
        }

        [HttpDelete("members/{id}")]
        public async Task<IActionResult> RemoveMember(string id)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var target = await _userService.GetByPublicIdAsync(id);
            if (target is null || target.IsDeleted)
                return NotFound();

            if (IsProtected(access.User!, target))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "This account cannot be removed here." });

            await _userService.DeleteAsync(target);
            return NoContent();
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts([FromQuery] string? q)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var posts = await _postService.GetFeedAsync();
            var term = q?.Trim();
            var response = posts
                .Where(post => !post.IsDeleted)
                .Where(post => Matches(term, post.Text, post.Author?.Name, post.Author?.Username))
                .OrderByDescending(post => post.CreatedAt)
                .Select(ToPostDto)
                .ToList();

            return Ok(response);
        }

        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> RemovePost(string id)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var post = await _postService.GetTrackedByPublicIdAsync(id);
            if (post is null || post.IsDeleted)
                return NotFound();

            await _postService.DeleteAsync(post);
            await CloseOpenReportsAsync(post.Id, ReportStatus.Removed);
            return NoContent();
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var reports = await _reportService.GetOpenAsync();
            var response = reports
                .Where(report => !report.IsDeleted)
                .OrderByDescending(report => report.CreatedAt)
                .Select(ToReportDto)
                .ToList();

            return Ok(response);
        }

        [HttpPost("reports/{id}/dismiss")]
        public async Task<IActionResult> DismissReport(string id)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var report = await _reportService.GetOpenByPublicIdAsync(id);
            if (report is null || report.IsDeleted)
                return NotFound();

            report.Status = ReportStatus.Dismissed;
            await _reportService.EditAsync(report);

            var updated = await _reportService.GetByPublicIdAsync(report.PublicId);
            return Ok(updated is null ? ToReportDto(report) : ToReportDto(updated));
        }

        [HttpPost("reports/{id}/remove")]
        public async Task<IActionResult> RemoveReportedPost(string id)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var report = await _reportService.GetOpenByPublicIdAsync(id);
            if (report is null || report.IsDeleted)
                return NotFound();

            if (report.Post is not null && !report.Post.IsDeleted)
                await _postService.DeleteAsync(report.Post);

            await CloseOpenReportsAsync(report.PostId, ReportStatus.Removed);

            var updated = await _reportService.GetByPublicIdAsync(report.PublicId);
            return Ok(updated is null ? ToReportDto(report) : ToReportDto(updated));
        }

        [HttpGet("communities")]
        public async Task<IActionResult> GetCommunities([FromQuery] string? q)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var communities = await _communityService.GetAllDetailedAsync();
            var term = q?.Trim();
            var response = communities
                .Where(community => !community.IsDeleted)
                .Where(community => Matches(term, community.Name, community.Description))
                .Select(community => ToCommunityDto(community, access.User!.Id))
                .ToList();

            return Ok(response);
        }

        [HttpPut("communities/{id}")]
        public async Task<IActionResult> UpdateCommunity(string id, [FromBody] CreateCommunityRequest request)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var community = await _communityService.GetTrackedByPublicIdAsync(id);
            if (community is null || community.IsDeleted)
                return NotFound();

            var error = await _communityValidator.FirstErrorAsync(request, CreateCommunityRequestValidator.Admin);
            if (error is not null)
                return BadRequest(new { message = error });

            ApplyCommunity(request, community);

            await _communityService.EditAsync(community);
            var detailed = await _communityService.GetDetailedByPublicIdAsync(community.PublicId);
            return Ok(ToCommunityDto(detailed ?? community, access.User!.Id));
        }

        [HttpDelete("communities/{id}")]
        public async Task<IActionResult> RemoveCommunity(string id)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var community = await _communityService.GetTrackedByPublicIdAsync(id);
            if (community is null || community.IsDeleted)
                return NotFound();

            await _communityService.DeleteAsync(community);
            return NoContent();
        }

        [HttpGet("site")]
        public async Task<IActionResult> GetSite()
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var settings = await EnsureSiteSettingsAsync();
            return Ok(ToSiteDto(settings));
        }

        [HttpPut("site")]
        public async Task<IActionResult> UpdateSite([FromBody] UpdateSiteSettingsRequest request)
        {
            var access = await RequireAdminAsync();
            if (access.Error is not null)
                return access.Error;

            var error = await _siteValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var settings = await EnsureSiteSettingsAsync();
            ApplySite(settings, request);

            await _siteSettingsService.EditAsync(settings);
            return Ok(ToSiteDto(settings));
        }

        private async Task CloseOpenReportsAsync(int postId, ReportStatus status)
        {
            var openReports = await _reportService.GetOpenByPostIdAsync(postId);
            foreach (var openReport in openReports.Where(item => !item.IsDeleted))
            {
                openReport.Status = status;
                await _reportService.EditAsync(openReport);
            }
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

        private async Task<(AppUser? User, IActionResult? Error)> RequireAdminAsync()
        {
            var publicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(publicId))
                return (null, Unauthorized());

            var user = await _userService.GetByPublicIdAsync(publicId);
            if (user is null || user.IsDeleted || !user.IsActive)
                return (null, Unauthorized());

            if (user.Role is not (UserRole.Admin or UserRole.SuperAdmin))
                return (null, StatusCode(StatusCodes.Status403Forbidden, new { message = "Only an admin can use the admin console." }));

            return (user, null);
        }

        private static bool IsProtected(AppUser actor, AppUser target)
        {
            if (target.Role == UserRole.SuperAdmin || target.Id == actor.Id)
                return true;

            return target.Role == UserRole.Admin && actor.Role != UserRole.SuperAdmin;
        }

        private static UserRole ParseRole(string? value)
        {
            return value?.Trim().ToLowerInvariant() == "admin" ? UserRole.Admin : UserRole.Member;
        }

        private static bool Matches(string? term, params string?[] values)
        {
            if (string.IsNullOrWhiteSpace(term))
                return true;

            return values.Any(value => value?.Contains(term, StringComparison.OrdinalIgnoreCase) == true);
        }

        private static void ApplyCommunity(CreateCommunityRequest request, Community community)
        {
            var name = request.Name.Trim();
            var color = request.Color?.Trim() ?? string.Empty;
            community.Name = name;
            community.Description = request.Description?.Trim() ?? string.Empty;
            community.Emoji = NormalizeEmoji(request.Emoji, name);
            if (color.Length > 0)
                community.Color = color;

            if (request.Banner is not null)
                community.BannerUrl = request.Banner.Trim();
        }

        private static string NormalizeEmoji(string? emoji, string name)
        {
            var value = emoji?.Trim();
            if (string.IsNullOrWhiteSpace(value))
                return name.Length == 0 ? "✳" : name[..1].ToUpperInvariant();

            return value.Length <= 8 ? value : value[..8];
        }

        private static void ApplySite(SiteSettings settings, UpdateSiteSettingsRequest request)
        {
            if (request.SiteName is not null)
                settings.SiteName = request.SiteName.Trim();

            if (request.Tagline is not null)
                settings.Tagline = request.Tagline.Trim();

            if (request.LogoMark is not null)
            {
                var mark = request.LogoMark.Trim();
                settings.LogoMark = mark.Length == 0 ? FirstMark(settings.SiteName) : mark;
            }

            if (request.FooterNote is not null)
                settings.FooterNote = request.FooterNote.Trim();

            if (request.LogoUrl is not null)
                settings.LogoUrl = request.LogoUrl.Trim();

            if (request.AboutBody is not null)
                settings.AboutBody = request.AboutBody.Trim();

            if (request.GuidelinesBody is not null)
                settings.GuidelinesBody = request.GuidelinesBody.Trim();

            if (request.PrivacyBody is not null)
                settings.PrivacyBody = request.PrivacyBody.Trim();
        }

        private static string FirstMark(string siteName)
        {
            var name = siteName.Trim();
            return name.Length == 0 ? "g" : name[..1];
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

        private static AdminPostDto ToPostDto(Post post)
        {
            var communityName = post.Community?.Name
                ?? post.PostCommunities
                    .Where(link => !link.IsDeleted && link.Community is not null)
                    .Select(link => link.Community.Name)
                    .FirstOrDefault();

            return new AdminPostDto
            {
                Id = post.PublicId,
                AuthorId = post.Author?.PublicId ?? string.Empty,
                AuthorName = post.Author?.Name ?? string.Empty,
                AuthorUsername = post.Author?.Username ?? string.Empty,
                CommunityName = communityName,
                Text = post.Text,
                Privacy = ToPrivacy(post.Privacy),
                CreatedAt = post.CreatedAt
            };
        }

        private static ReportDto ToReportDto(Report report)
        {
            return new ReportDto
            {
                Id = report.PublicId,
                PostId = report.Post?.PublicId ?? string.Empty,
                ReporterId = report.Reporter?.PublicId ?? string.Empty,
                Reason = report.Reason,
                CreatedAt = report.CreatedAt,
                Status = ToReportStatus(report.Status),
                PostText = report.Post?.Text,
                AuthorName = report.Post?.Author?.Name,
                AuthorUsername = report.Post?.Author?.Username,
                ReporterName = report.Reporter?.Name
            };
        }

        private static CoomunityDto ToCommunityDto(Community community, int currentUserId)
        {
            var members = (community.Members ?? [])
                .Where(member => !member.IsDeleted && member.User is { IsDeleted: false })
                .GroupBy(member => member.UserId)
                .Select(group => group.First())
                .ToList();

            return new CoomunityDto
            {
                Id = community.PublicId,
                Name = community.Name,
                Emoji = community.Emoji,
                Description = community.Description,
                Members = members.Count > 0 ? members.Count : community.MemberCount,
                MemberIds = members.Select(member => member.User.PublicId).ToList(),
                OwnerId = community.Owner?.PublicId ?? string.Empty,
                Joined = members.Any(member => member.UserId == currentUserId),
                Color = community.Color,
                Banner = community.BannerUrl
            };
        }

        private static SiteSettingsDto ToSiteDto(SiteSettings settings)
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

        private static string ToRole(UserRole role)
        {
            return role switch
            {
                UserRole.SuperAdmin => "superadmin",
                UserRole.Admin => "admin",
                _ => "member"
            };
        }

        private static string ToPrivacy(PostPrivacy privacy)
        {
            return privacy switch
            {
                PostPrivacy.Friends => "friends",
                PostPrivacy.Communities => "communities",
                _ => "public"
            };
        }

        private static string ToReportStatus(ReportStatus status)
        {
            return status switch
            {
                ReportStatus.Dismissed => "dismissed",
                ReportStatus.Removed => "removed",
                _ => "open"
            };
        }
    }
}
