using System.Security.Claims;
using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.FriendRequest;
using Service.Helpers.DTOs.Friendship;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IFriendRequestService _friendRequestService;
        private readonly INotificationService _notificationService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IAppUserService _userService;

        public FriendsController(
            IFriendshipService friendshipService,
            IFriendRequestService friendRequestService,
            INotificationService notificationService,
            IUserSettingsService userSettingsService,
            IAppUserService userService)
        {
            _friendshipService = friendshipService;
            _friendRequestService = friendRequestService;
            _notificationService = notificationService;
            _userSettingsService = userSettingsService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var users = await UsersByIdAsync();
            var friendships = await _friendshipService.GetAllAsync();
            var response = friendships
                .Where(friendship => !friendship.IsDeleted && Involves(friendship, currentUser.Id))
                .Select(friendship => ToFriend(friendship, currentUser, users))
                .Where(friendship => friendship is not null)
                .Cast<FriendshipDto>()
                .GroupBy(friendship => friendship.FriendId)
                .Select(group => group.OrderBy(friendship => friendship.CreatedAt).First())
                .OrderBy(friendship => friendship.CreatedAt)
                .ToList();

            return Ok(response);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetIncoming()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var users = await UsersByIdAsync();
            var requests = await _friendRequestService.GetAllAsync();
            var response = requests
                .Where(request => !request.IsDeleted && request.Status == RequestStatus.Pending && request.ToUserId == currentUser.Id)
                .Select(request => ToRequest(request, users))
                .Where(request => request is not null)
                .Cast<FriendRequestDto>()
                .OrderByDescending(request => request.CreatedAt)
                .ToList();

            return Ok(response);
        }

        [HttpGet("requests/outgoing")]
        public async Task<IActionResult> GetOutgoing()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var users = await UsersByIdAsync();
            var requests = await _friendRequestService.GetAllAsync();
            var response = requests
                .Where(request => !request.IsDeleted && request.Status == RequestStatus.Pending && request.FromUserId == currentUser.Id)
                .Select(request => ToRequest(request, users))
                .Where(request => request is not null)
                .Cast<FriendRequestDto>()
                .OrderByDescending(request => request.CreatedAt)
                .ToList();

            return Ok(response);
        }

        [HttpPost("requests/{userId}")]
        public async Task<IActionResult> Send(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var target = await _userService.GetByPublicIdAsync(userId);
            if (target is null || target.IsDeleted || !target.IsActive)
                return NotFound();

            if (target.Id == currentUser.Id)
                return BadRequest(new { message = "You cannot send a friend request to yourself." });

            if (await IsBlockedAsync(currentUser, target))
                return BadRequest(new { message = "You cannot send a friend request to this person." });

            if (await AreFriendsAsync(currentUser.Id, target.Id))
                return BadRequest(new { message = "You are already friends." });

            var settings = await _userSettingsService.GetByUserIdAsync(target.Id);
            if (settings is { AllowRequests: false })
                return BadRequest(new { message = "This person is not accepting friend requests." });

            var incoming = await _friendRequestService.GetPendingAsync(target.Id, currentUser.Id);
            if (incoming is not null)
                return BadRequest(new { message = "This person already sent you a friend request." });

            var request = await _friendRequestService.FindAsync(currentUser.Id, target.Id);
            if (request is { IsDeleted: false, Status: RequestStatus.Pending })
            {
                await OpenRequestNotificationAsync(target, currentUser);
                return Ok(ToRequest(request, currentUser, target));
            }

            if (request is null)
            {
                request = new FriendRequest
                {
                    FromUserId = currentUser.Id,
                    ToUserId = target.Id,
                    Status = RequestStatus.Pending
                };

                try
                {
                    await _friendRequestService.CreateAsync(request);
                }
                catch (DbUpdateException)
                {
                    request = await _friendRequestService.FindAsync(currentUser.Id, target.Id);
                    if (request is null)
                        return Conflict(new { message = "That friend request could not be sent." });
                }
            }

            request.IsDeleted = false;
            request.Status = RequestStatus.Pending;
            await _friendRequestService.EditAsync(request);
            await OpenRequestNotificationAsync(target, currentUser);
            return Ok(ToRequest(request, currentUser, target));
        }

        [HttpDelete("requests/{userId}")]
        public async Task<IActionResult> Cancel(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var target = await _userService.GetByPublicIdAsync(userId);
            if (target is null || target.IsDeleted)
                return NotFound();

            var request = await _friendRequestService.GetPendingAsync(currentUser.Id, target.Id);
            if (request is null)
                return NotFound();

            await _friendRequestService.DeleteAsync(request);
            await CloseRequestNotificationAsync(target.Id, currentUser.Id, RequestStatus.Declined);
            return NoContent();
        }

        [HttpPost("requests/{userId}/accept")]
        public Task<IActionResult> Accept(string userId)
        {
            return RespondAsync(userId, accept: true);
        }

        [HttpPost("requests/{userId}/decline")]
        public Task<IActionResult> Decline(string userId)
        {
            return RespondAsync(userId, accept: false);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Remove(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var target = await _userService.GetByPublicIdAsync(userId);
            if (target is null || target.IsDeleted)
                return NotFound();

            var friendships = await _friendshipService.GetAllAsync();
            var rows = friendships.Where(friendship => !friendship.IsDeleted && Involves(friendship, currentUser.Id, target.Id)).ToList();
            if (rows.Count == 0)
                return NotFound();

            foreach (var row in rows)
                await _friendshipService.DeleteAsync(row);

            await CloseAcceptedRequestAsync(currentUser.Id, target.Id);
            await CloseAcceptedRequestAsync(target.Id, currentUser.Id);
            return NoContent();
        }

        private async Task<IActionResult> RespondAsync(string userId, bool accept)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var target = await _userService.GetByPublicIdAsync(userId);
            if (target is null || target.IsDeleted || !target.IsActive)
                return NotFound();

            var request = await _friendRequestService.GetPendingAsync(target.Id, currentUser.Id);
            if (request is null)
                return NotFound();

            var status = accept ? RequestStatus.Accepted : RequestStatus.Declined;
            request.Status = status;
            await _friendRequestService.EditAsync(request);

            if (accept)
            {
                await EnsureFriendshipAsync(currentUser.Id, target.Id);
                await EnsureFriendshipAsync(target.Id, currentUser.Id);
            }

            await CloseRequestNotificationAsync(currentUser.Id, target.Id, status);
            return Ok(ToRequest(request, target, currentUser));
        }

        private async Task EnsureFriendshipAsync(int userId, int friendId)
        {
            var existing = await _friendshipService.FindAsync(userId, friendId);
            if (existing is { IsDeleted: false })
                return;

            if (existing is null)
            {
                try
                {
                    await _friendshipService.CreateAsync(new Friendship
                    {
                        UserId = userId,
                        FriendId = friendId
                    });
                    return;
                }
                catch (DbUpdateException)
                {
                    existing = await _friendshipService.FindAsync(userId, friendId);
                    if (existing is null || !existing.IsDeleted)
                        return;
                }
            }

            existing.IsDeleted = false;
            await _friendshipService.EditAsync(existing);
        }

        private async Task<bool> AreFriendsAsync(int firstId, int secondId)
        {
            return await _friendshipService.ExistsAsync(firstId, secondId)
                || await _friendshipService.ExistsAsync(secondId, firstId);
        }

        private async Task OpenRequestNotificationAsync(AppUser recipient, AppUser actor)
        {
            var settings = await _userSettingsService.GetByUserIdAsync(recipient.Id);
            if (settings is { NotifyRequests: false })
                return;

            var notifications = await _notificationService.GetForRecipientAsync(recipient.Id);
            var existing = notifications
                .Where(notification => !notification.IsDeleted && notification.Type == NotificationType.FriendRequest && notification.ActorId == actor.Id)
                .OrderByDescending(notification => notification.CreatedAt)
                .FirstOrDefault();

            if (existing is null)
            {
                await _notificationService.CreateAsync(new Notification
                {
                    PublicId = Guid.NewGuid().ToString(),
                    RecipientId = recipient.Id,
                    ActorId = actor.Id,
                    Type = NotificationType.FriendRequest,
                    Detail = string.Empty,
                    IsRead = false,
                    Status = RequestStatus.Pending
                });
                return;
            }

            var tracked = await _notificationService.GetByPublicIdAsync(existing.PublicId, recipient.Id);
            if (tracked is null || tracked.IsDeleted)
                return;

            tracked.Status = RequestStatus.Pending;
            tracked.IsRead = false;
            await _notificationService.EditAsync(tracked);
        }

        private async Task CloseRequestNotificationAsync(int recipientId, int actorId, RequestStatus status)
        {
            var notifications = await _notificationService.GetForRecipientAsync(recipientId);
            var matches = notifications.Where(notification =>
                !notification.IsDeleted
                && notification.Type == NotificationType.FriendRequest
                && notification.ActorId == actorId
                && notification.Status == RequestStatus.Pending);

            foreach (var notification in matches)
            {
                var tracked = await _notificationService.GetByPublicIdAsync(notification.PublicId, recipientId);
                if (tracked is null || tracked.IsDeleted)
                    continue;

                tracked.Status = status;
                tracked.IsRead = true;
                await _notificationService.EditAsync(tracked);
            }
        }

        private async Task CloseAcceptedRequestAsync(int fromUserId, int toUserId)
        {
            var request = await _friendRequestService.FindAsync(fromUserId, toUserId);
            if (request is null || request.IsDeleted || request.Status != RequestStatus.Accepted)
                return;

            request.Status = RequestStatus.Declined;
            await _friendRequestService.EditAsync(request);
        }

        private async Task<bool> IsBlockedAsync(AppUser currentUser, AppUser target)
        {
            var currentBlocked = await GetBlockedIdsAsync(currentUser.Id);
            var targetBlocked = await GetBlockedIdsAsync(target.Id);
            return currentBlocked.Contains(target.PublicId) || targetBlocked.Contains(currentUser.PublicId);
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

        private async Task<Dictionary<int, AppUser>> UsersByIdAsync()
        {
            var users = await _userService.GetAllAsync();
            return users.ToDictionary(user => user.Id);
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

        private static bool Involves(Friendship friendship, int userId)
        {
            return friendship.UserId == userId || friendship.FriendId == userId;
        }

        private static bool Involves(Friendship friendship, int firstId, int secondId)
        {
            return (friendship.UserId == firstId && friendship.FriendId == secondId)
                || (friendship.UserId == secondId && friendship.FriendId == firstId);
        }

        private static FriendshipDto? ToFriend(Friendship friendship, AppUser currentUser, IReadOnlyDictionary<int, AppUser> users)
        {
            var friendId = friendship.UserId == currentUser.Id ? friendship.FriendId : friendship.UserId;
            if (!users.TryGetValue(friendId, out var friend) || friend.IsDeleted || !friend.IsActive)
                return null;

            return new FriendshipDto
            {
                UserId = currentUser.PublicId,
                FriendId = friend.PublicId,
                CreatedAt = friendship.CreatedAt
            };
        }

        private static FriendRequestDto? ToRequest(FriendRequest request, IReadOnlyDictionary<int, AppUser> users)
        {
            if (!users.TryGetValue(request.FromUserId, out var fromUser) || fromUser.IsDeleted)
                return null;

            if (!users.TryGetValue(request.ToUserId, out var toUser) || toUser.IsDeleted)
                return null;

            return ToRequest(request, fromUser, toUser);
        }

        private static FriendRequestDto ToRequest(FriendRequest request, AppUser fromUser, AppUser toUser)
        {
            return new FriendRequestDto
            {
                FromUserId = fromUser.PublicId,
                ToUserId = toUser.PublicId,
                Status = ToStatus(request.Status),
                CreatedAt = request.CreatedAt
            };
        }

        private static string ToStatus(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => "pending",
                RequestStatus.Accepted => "accepted",
                RequestStatus.Declined => "declined",
                _ => "done"
            };
        }
    }
}
