using System.Security.Claims;
using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.DTOs.Report;
using Service.Helpers.Validators;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IPostService _postService;
        private readonly IFriendshipService _friendshipService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IAppUserService _userService;
        private readonly IValidator<CreateReportRequest> _reportValidator;

        public ReportsController(
            IReportService reportService,
            IPostService postService,
            IFriendshipService friendshipService,
            IUserSettingsService userSettingsService,
            IAppUserService userService,
            IValidator<CreateReportRequest> reportValidator)
        {
            _reportService = reportService;
            _postService = postService;
            _friendshipService = friendshipService;
            _userSettingsService = userSettingsService;
            _userService = userService;
            _reportValidator = reportValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetOpen()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var reports = await _reportService.GetOpenAsync(IsAdmin(currentUser) ? null : currentUser.Id);
            var response = reports
                .Where(report => !report.IsDeleted)
                .Select(ToDto)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var report = await _reportService.GetByPublicIdAsync(id);
            if (report is null || report.IsDeleted || !CanRead(report, currentUser))
                return NotFound();

            return Ok(ToDto(report));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var error = await _reportValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var reason = request.Reason.Trim();
            var extra = request.Details?.Trim() ?? string.Empty;
            if (extra.Length > 0)
                reason = $"{reason} — {extra}";

            var post = await GetVisiblePostAsync(request.PostId, currentUser);
            if (post is null)
                return NotFound();

            if (post.AuthorId == currentUser.Id)
                return BadRequest(new { message = "You cannot report your own post." });

            if (await _reportService.HasOpenAsync(post.Id, currentUser.Id))
            {
                var existing = (await _reportService.GetOpenAsync(currentUser.Id))
                    .FirstOrDefault(report => !report.IsDeleted && report.PostId == post.Id);
                if (existing is not null)
                    return Ok(ToDto(existing));
            }

            var report = new Report
            {
                PublicId = Guid.NewGuid().ToString(),
                PostId = post.Id,
                ReporterId = currentUser.Id,
                Reason = reason,
                Status = ReportStatus.Open
            };

            await _reportService.CreateAsync(report);
            var created = await _reportService.GetByPublicIdAsync(report.PublicId);
            return Ok(created is null ? ToDto(report, post, currentUser) : ToDto(created));
        }

        [HttpPost("{id}/dismiss")]
        public async Task<IActionResult> Dismiss(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            if (!IsAdmin(currentUser))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only an admin can review reports." });

            var report = await _reportService.GetOpenByPublicIdAsync(id);
            if (report is null || report.IsDeleted)
                return NotFound();

            report.Status = ReportStatus.Dismissed;
            await _reportService.EditAsync(report);

            var updated = await _reportService.GetByPublicIdAsync(report.PublicId);
            return Ok(updated is null ? ToDto(report) : ToDto(updated));
        }

        [HttpPost("{id}/remove")]
        public async Task<IActionResult> RemovePost(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            if (!IsAdmin(currentUser))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only an admin can review reports." });

            var report = await _reportService.GetOpenByPublicIdAsync(id);
            if (report is null || report.IsDeleted)
                return NotFound();

            if (report.Post is not null && !report.Post.IsDeleted)
                await _postService.DeleteAsync(report.Post);

            var openReports = await _reportService.GetOpenByPostIdAsync(report.PostId);
            foreach (var openReport in openReports.Where(item => !item.IsDeleted))
            {
                openReport.Status = ReportStatus.Removed;
                await _reportService.EditAsync(openReport);
            }

            var updated = await _reportService.GetByPublicIdAsync(report.PublicId);
            return Ok(updated is null ? ToDto(report) : ToDto(updated));
        }

        private async Task<Post?> GetVisiblePostAsync(string publicId, AppUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return null;

            var post = await _postService.GetByPublicIdAsync(publicId);
            if (post is null || post.IsDeleted)
                return null;

            var detailed = await _postService.GetDetailedByIdAsync(post.Id);
            if (detailed is null || detailed.IsDeleted)
                return null;

            var blockedIds = await GetBlockedIdsAsync(currentUser.Id);
            var friendIds = (await _friendshipService.GetFriendPublicIdsAsync(currentUser.Id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!await CanSeeAsync(detailed, currentUser, blockedIds, friendIds))
                return null;

            return detailed;
        }

        private async Task<bool> CanSeeAsync(Post post, AppUser currentUser, IReadOnlySet<string> blockedIds, IReadOnlySet<string> friendIds)
        {
            if (post.Author is { IsActive: false } or { IsDeleted: true })
                return false;

            if (post.AuthorId != currentUser.Id
                && post.Author is not null
                && blockedIds.Contains(post.Author.PublicId))
                return false;

            if (post.AuthorId == currentUser.Id || post.Privacy == PostPrivacy.Public)
                return true;

            if (post.Privacy == PostPrivacy.Friends)
            {
                return post.Author is not null && friendIds.Contains(post.Author.PublicId)
                    || await _friendshipService.ExistsAsync(post.AuthorId, currentUser.Id);
            }

            return post.Privacy == PostPrivacy.Communities;
        }

        private async Task<HashSet<string>> GetBlockedIdsAsync(int userId)
        {
            var settings = await _userSettingsService.GetByUserIdAsync(userId);
            if (settings is null || string.IsNullOrWhiteSpace(settings.BlockedUserPublicIds))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var ids = JsonSerializer.Deserialize<List<string>>(settings.BlockedUserPublicIds);
                return ids?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            catch (JsonException)
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
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

        private static bool IsAdmin(AppUser user)
        {
            return user.Role is UserRole.Admin or UserRole.SuperAdmin;
        }

        private static bool CanRead(Report report, AppUser currentUser)
        {
            return IsAdmin(currentUser) || report.ReporterId == currentUser.Id;
        }

        private static ReportDto ToDto(Report report)
        {
            return new ReportDto
            {
                Id = report.PublicId,
                PostId = report.Post?.PublicId ?? string.Empty,
                ReporterId = report.Reporter?.PublicId ?? string.Empty,
                Reason = report.Reason,
                CreatedAt = report.CreatedAt,
                Status = ToStatus(report.Status),
                PostText = report.Post?.Text,
                AuthorName = report.Post?.Author?.Name,
                AuthorUsername = report.Post?.Author?.Username,
                ReporterName = report.Reporter?.Name
            };
        }

        private static ReportDto ToDto(Report report, Post post, AppUser reporter)
        {
            return new ReportDto
            {
                Id = report.PublicId,
                PostId = post.PublicId,
                ReporterId = reporter.PublicId,
                Reason = report.Reason,
                CreatedAt = report.CreatedAt,
                Status = ToStatus(report.Status),
                PostText = post.Text,
                AuthorName = post.Author?.Name,
                AuthorUsername = post.Author?.Username,
                ReporterName = reporter.Name
            };
        }

        private static string ToStatus(ReportStatus status)
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
