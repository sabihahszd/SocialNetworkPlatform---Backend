using System.Security.Claims;
using System.Text.RegularExpressions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.Community;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class CommunitiesController : ControllerBase
    {
        private const int MaxNameLength = 32;
        private const int MaxDescriptionLength = 120;

        private readonly ICommunityService _communityService;
        private readonly ICommunityMemberService _communityMemberService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly INotificationService _notificationService;
        private readonly IAppUserService _userService;

        public CommunitiesController(
            ICommunityService communityService,
            ICommunityMemberService communityMemberService,
            IUserSettingsService userSettingsService,
            INotificationService notificationService,
            IAppUserService userService)
        {
            _communityService = communityService;
            _communityMemberService = communityMemberService;
            _userSettingsService = userSettingsService;
            _notificationService = notificationService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var communities = await _communityService.GetAllDetailedAsync();
            var response = communities
                .Where(community => !community.IsDeleted)
                .Select(community => ToDto(community, currentUser.Id))
                .ToList();

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var matches = await _communityService.SearchAsync(q);
            var response = new List<CoomunityDto>();
            foreach (var match in matches.Where(community => !community.IsDeleted))
            {
                var community = await _communityService.GetDetailedByPublicIdAsync(match.PublicId);
                if (community is not null && !community.IsDeleted)
                    response.Add(ToDto(community, currentUser.Id));
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var community = await GetVisibleCommunityAsync(id);
            if (community is null)
                return NotFound();

            return Ok(ToDto(community, currentUser.Id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommunityRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            if (!TryNormalizeName(request.Name, out var name, out var error))
                return BadRequest(new { message = error });

            if (!TryNormalizeDescription(request.Description, out var description, out error))
                return BadRequest(new { message = error });

            var community = new Community
            {
                PublicId = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                Emoji = NormalizeEmoji(request.Emoji, name),
                Color = NormalizeColor(request.Color),
                BannerUrl = request.Banner?.Trim() ?? string.Empty,
                OwnerId = currentUser.Id,
                MemberCount = 1
            };

            await _communityService.CreateAsync(community);
            await AddMemberAsync(community.Id, currentUser.Id);

            var detailed = await _communityService.GetDetailedByPublicIdAsync(community.PublicId);
            return Ok(ToDto(detailed ?? community, currentUser.Id));
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> ToggleJoin(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var community = await _communityService.GetTrackedByPublicIdAsync(id);
            if (community is null || community.IsDeleted)
                return NotFound();

            var membership = await _communityMemberService.FindAsync(community.Id, currentUser.Id);
            var joined = membership is not null && !membership.IsDeleted;
            if (joined)
            {
                if (community.OwnerId == currentUser.Id)
                    return BadRequest(new { message = "The creator stays in the community." });

                await _communityMemberService.DeleteAsync(membership!);
            }
            else
            {
                await AddMemberAsync(community.Id, currentUser.Id);
                await NotifyOwnerAsync(community, currentUser);
            }

            var detailed = await _communityService.GetDetailedByPublicIdAsync(community.PublicId);
            var memberCount = detailed?.Members.Count(member => !member.IsDeleted) ?? community.MemberCount;
            if (community.MemberCount != memberCount)
            {
                community.MemberCount = memberCount;
                await _communityService.EditAsync(community);
            }

            if (detailed is not null)
                detailed.MemberCount = memberCount;

            return Ok(ToDto(detailed ?? community, currentUser.Id));
        }

        private async Task AddMemberAsync(int communityId, int userId)
        {
            var membership = await _communityMemberService.FindAsync(communityId, userId);
            if (membership is null)
            {
                try
                {
                    await _communityMemberService.CreateAsync(new CommunityMember
                    {
                        CommunityId = communityId,
                        UserId = userId
                    });
                }
                catch (DbUpdateException)
                {
                    membership = await _communityMemberService.FindAsync(communityId, userId);
                    if (membership is { IsDeleted: true })
                    {
                        membership.IsDeleted = false;
                        await _communityMemberService.EditAsync(membership);
                    }
                }

                return;
            }

            if (membership.IsDeleted)
            {
                membership.IsDeleted = false;
                await _communityMemberService.EditAsync(membership);
            }
        }

        private async Task NotifyOwnerAsync(Community community, AppUser actor)
        {
            if (community.OwnerId == actor.Id)
                return;

            var settings = await _userSettingsService.GetByUserIdAsync(community.OwnerId);
            if (settings is { NotifyCommunities: false })
                return;

            await _notificationService.CreateAsync(new Notification
            {
                PublicId = Guid.NewGuid().ToString(),
                RecipientId = community.OwnerId,
                ActorId = actor.Id,
                Type = NotificationType.Community,
                Detail = community.Name,
                IsRead = false,
                Status = RequestStatus.Done
            });
        }

        private async Task<Community?> GetVisibleCommunityAsync(string publicId)
        {
            var community = await _communityService.GetDetailedByPublicIdAsync(publicId);
            if (community is null || community.IsDeleted)
                return null;

            return community;
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

        private static bool TryNormalizeName(string? value, out string name, out string error)
        {
            name = value?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                error = "Give the community a name.";
                return false;
            }

            if (name.Length > MaxNameLength)
            {
                error = $"Community names can be up to {MaxNameLength} characters.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryNormalizeDescription(string? value, out string description, out string error)
        {
            description = value?.Trim() ?? string.Empty;
            if (description.Length > MaxDescriptionLength)
            {
                error = $"Descriptions can be up to {MaxDescriptionLength} characters.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static string NormalizeEmoji(string? emoji, string name)
        {
            var value = emoji?.Trim();
            if (string.IsNullOrWhiteSpace(value))
                return name.Length == 0 ? "✳" : name[..1].ToUpperInvariant();

            return value.Length <= 8 ? value : value[..8];
        }

        private static string NormalizeColor(string? color)
        {
            var value = color?.Trim();
            if (!string.IsNullOrWhiteSpace(value) && ColorPattern().IsMatch(value))
                return value;

            return "#e8efdc";
        }

        private static CoomunityDto ToDto(Community community, int currentUserId)
        {
            var members = community.Members
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
                Members = members.Count,
                MemberIds = members.Select(member => member.User.PublicId).ToList(),
                OwnerId = community.Owner?.PublicId ?? string.Empty,
                Joined = members.Any(member => member.UserId == currentUserId),
                Color = community.Color,
                Banner = community.BannerUrl
            };
        }

        [GeneratedRegex("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$")]
        private static partial Regex ColorPattern();
    }
}
