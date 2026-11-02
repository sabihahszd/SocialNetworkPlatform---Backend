using System.Security.Claims;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.Notification;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IFriendRequestService _friendRequestService;
        private readonly IFriendshipService _friendshipService;
        private readonly IAppUserService _userService;

        public NotificationsController(
            INotificationService notificationService,
            IFriendRequestService friendRequestService,
            IFriendshipService friendshipService,
            IAppUserService userService)
        {
            _notificationService = notificationService;
            _friendRequestService = friendRequestService;
            _friendshipService = friendshipService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var notifications = await _notificationService.GetForRecipientAsync(currentUser.Id);
            var response = notifications
                .Where(notification => !notification.IsDeleted)
                .Select(ToDto)
                .ToList();

            return Ok(response);
        }

        [HttpPut("read")]
        public async Task<IActionResult> MarkAllRead()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var unread = await _notificationService.GetUnreadAsync(currentUser.Id);
            foreach (var notification in unread.Where(notification => !notification.IsDeleted))
            {
                notification.IsRead = true;
                await _notificationService.EditAsync(notification);
            }

            return NoContent();
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var notification = await GetOwnNotificationAsync(id, currentUser.Id);
            if (notification is null)
                return NotFound();

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _notificationService.EditAsync(notification);
            }

            return Ok(ToDto(notification));
        }

        [HttpPost("{id}/accept")]
        public Task<IActionResult> Accept(string id)
        {
            return RespondToRequestAsync(id, accept: true);
        }

        [HttpPost("{id}/decline")]
        public Task<IActionResult> Decline(string id)
        {
            return RespondToRequestAsync(id, accept: false);
        }

        private async Task<IActionResult> RespondToRequestAsync(string id, bool accept)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var notification = await GetOwnNotificationAsync(id, currentUser.Id);
            if (notification is null)
                return NotFound();

            if (notification.Type != NotificationType.FriendRequest)
                return BadRequest(new { message = "That notification is not a friend request." });

            var targetStatus = accept ? RequestStatus.Accepted : RequestStatus.Declined;
            if (notification.Status == targetStatus)
                return Ok(ToDto(notification));

            if (notification.Status != RequestStatus.Pending)
                return BadRequest(new { message = "This request is no longer pending." });

            if (notification.ActorId == currentUser.Id)
                return BadRequest(new { message = "You cannot respond to your own friend request." });

            var request = await _friendRequestService.FindAsync(notification.ActorId, currentUser.Id);
            if (request is not null)
            {
                if (request.Status == RequestStatus.Accepted && !accept)
                    return BadRequest(new { message = "You are already friends." });

                if (request.Status == RequestStatus.Declined && accept)
                    return BadRequest(new { message = "This request was already declined." });

                request.IsDeleted = false;
                request.Status = targetStatus;
                await _friendRequestService.EditAsync(request);
            }

            if (accept)
            {
                await EnsureFriendshipAsync(currentUser.Id, notification.ActorId);
                await EnsureFriendshipAsync(notification.ActorId, currentUser.Id);
            }

            notification.Status = targetStatus;
            notification.IsRead = true;
            await _notificationService.EditAsync(notification);
            return Ok(ToDto(notification));
        }

        private async Task EnsureFriendshipAsync(int userId, int friendId)
        {
            if (await _friendshipService.ExistsAsync(userId, friendId))
                return;

            try
            {
                await _friendshipService.CreateAsync(new Friendship
                {
                    UserId = userId,
                    FriendId = friendId
                });
            }
            catch (DbUpdateException)
            {
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

        private async Task<Notification?> GetOwnNotificationAsync(string publicId, int recipientId)
        {
            var notification = await _notificationService.GetByPublicIdAsync(publicId, recipientId);
            if (notification is null || notification.IsDeleted)
                return null;

            return notification;
        }

        private static NotificationDto ToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.PublicId,
                Type = ToType(notification.Type),
                ActorId = notification.Actor?.PublicId ?? string.Empty,
                PostId = notification.PostPublicId ?? string.Empty,
                CreatedAt = notification.CreatedAt,
                Read = notification.IsRead,
                Status = ToStatus(notification.Status),
                Detail = string.IsNullOrWhiteSpace(notification.Detail) ? null : notification.Detail
            };
        }

        private static string ToType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Like => "like",
                NotificationType.Comment => "comment",
                NotificationType.FriendRequest => "friend-request",
                NotificationType.Repost => "repost",
                NotificationType.Community => "community",
                _ => "comment"
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
