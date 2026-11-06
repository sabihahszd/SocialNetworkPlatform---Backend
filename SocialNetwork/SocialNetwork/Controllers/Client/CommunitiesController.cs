using System.Security.Claims;
using System.Text.RegularExpressions;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.Community;
using Service.Helpers.Validators;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class CommunitiesController : ControllerBase
    {
        private readonly ICommunityService _communityService;
        private readonly ICommunityMemberService _communityMemberService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly INotificationService _notificationService;
        private readonly IAppUserService _userService;
        private readonly IValidator<CreateCommunityRequest> _communityValidator;

        public CommunitiesController(
            ICommunityService communityService,
            ICommunityMemberService communityMemberService,
            IUserSettingsService userSettingsService,
            INotificationService notificationService,
            IAppUserService userService,
            IValidator<CreateCommunityRequest> communityValidator)
        {
            _communityService = communityService;
            _communityMemberService = communityMemberService;
            _userSettingsService = userSettingsService;
            _notificationService = notificationService;
            _userService = userService;
            _communityValidator = communityValidator;
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

            var error = await _communityValidator.FirstErrorAsync(request, CreateCommunityRequestValidator.Member);
            if (error is not null)
                return BadRequest(new { message = error });

            var name = request.Name.Trim();
            var description = request.Description?.Trim() ?? string.Empty;

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
            if (community.OwnerId == currentUser.Id)
            {
                if (!joined)
                    await AddMemberAsync(community.Id, currentUser.Id);
            }
            else if (joined)
            {
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

            var ownerId = community.Owner?.PublicId ?? string.Empty;
            var memberIds = members
                .Select(member => member.User.PublicId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();
            if (!string.IsNullOrWhiteSpace(ownerId) && !memberIds.Contains(ownerId))
                memberIds.Insert(0, ownerId);

            return new CoomunityDto
            {
                Id = community.PublicId,
                Name = community.Name,
                Emoji = community.Emoji,
                Description = community.Description,
                Members = memberIds.Count,
                MemberIds = memberIds,
                OwnerId = ownerId,
                Joined = community.OwnerId == currentUserId || members.Any(member => member.UserId == currentUserId),
                Color = community.Color,
                Banner = community.BannerUrl
            };
        }

        [GeneratedRegex("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$")]
        private static partial Regex ColorPattern();
    }
}
