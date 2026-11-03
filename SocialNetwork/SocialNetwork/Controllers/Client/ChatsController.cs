using System.Security.Claims;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.ChatMessage;
using Service.Helpers.DTOs.ChatThread;
using Service.Helpers.DTOs.Common;
using Service.Helpers.Validators;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatThreadService _chatThreadService;
        private readonly IChatMessageService _chatMessageService;
        private readonly IAppUserService _userService;
        private readonly IValidator<DirectMessageRequest> _directMessageValidator;
        private readonly IValidator<SendMessageRequest> _messageValidator;
        private readonly IValidator<UpdateTextRequest> _textValidator;

        public ChatsController(
            IChatThreadService chatThreadService,
            IChatMessageService chatMessageService,
            IAppUserService userService,
            IValidator<DirectMessageRequest> directMessageValidator,
            IValidator<SendMessageRequest> messageValidator,
            IValidator<UpdateTextRequest> textValidator)
        {
            _chatThreadService = chatThreadService;
            _chatMessageService = chatMessageService;
            _userService = userService;
            _directMessageValidator = directMessageValidator;
            _messageValidator = messageValidator;
            _textValidator = textValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var threads = await _chatThreadService.GetForUserAsync(currentUser.Id);
            var response = threads
                .Where(thread => !thread.IsDeleted)
                .OrderByDescending(LatestActivity)
                .Select(thread => ToThreadDto(thread, currentUser.Id))
                .ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var threads = await _chatThreadService.GetForUserAsync(currentUser.Id);
            var thread = threads.FirstOrDefault(item => item.PublicId == id && !item.IsDeleted);
            if (thread is null)
                return NotFound();

            return Ok(ToThreadDto(thread, currentUser.Id));
        }

        [HttpPost("direct")]
        public async Task<IActionResult> SendDirect([FromBody] DirectMessageRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var error = await _directMessageValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var text = request.Text.Trim();
            var participant = await _userService.GetByPublicIdAsync(request.ParticipantId.Trim());
            if (participant is null || participant.IsDeleted || !participant.IsActive)
                return NotFound();

            if (participant.Id == currentUser.Id)
                return BadRequest(new { message = "You cannot message yourself." });

            var thread = await _chatThreadService.GetBetweenAsync(currentUser.Id, participant.Id);
            if (thread is null)
            {
                thread = new ChatThread
                {
                    PublicId = Guid.NewGuid().ToString(),
                    UserAId = Math.Min(currentUser.Id, participant.Id),
                    UserBId = Math.Max(currentUser.Id, participant.Id)
                };

                try
                {
                    await _chatThreadService.CreateAsync(thread);
                }
                catch (DbUpdateException)
                {
                    thread = await _chatThreadService.GetBetweenAsync(currentUser.Id, participant.Id);
                    if (thread is null)
                        return Conflict(new { message = "That conversation could not be opened." });
                }
            }
            else if (thread.IsDeleted)
            {
                thread.IsDeleted = false;
                await _chatThreadService.EditAsync(thread);
            }

            var message = await CreateMessageAsync(thread.Id, currentUser.Id, text);
            return Ok(new DirectMessageDto
            {
                ThreadId = thread.PublicId,
                ParticipantId = participant.PublicId,
                Message = ToMessageDto(message, currentUser.PublicId)
            });
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> Send(string id, [FromBody] SendMessageRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var error = await _messageValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var text = request.Text.Trim();

            var thread = await GetMemberThreadAsync(id, currentUser.Id);
            if (thread is null)
                return NotFound();

            var message = await CreateMessageAsync(thread.Id, currentUser.Id, text);
            return Ok(ToMessageDto(message, currentUser.PublicId));
        }

        [HttpPut("{id}/messages/{messageId}")]
        public async Task<IActionResult> Edit(string id, string messageId, [FromBody] UpdateTextRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var error = await _textValidator.FirstErrorAsync(request, UpdateTextRequestValidator.Message);
            if (error is not null)
                return BadRequest(new { message = error });

            var text = request.Text.Trim();

            var message = await GetOwnMessageAsync(id, messageId, currentUser.Id);
            if (message is null)
                return NotFound();

            message.Text = text;
            await _chatMessageService.EditAsync(message);
            return Ok(ToMessageDto(message, currentUser.PublicId));
        }

        [HttpDelete("{id}/messages/{messageId}")]
        public async Task<IActionResult> Delete(string id, string messageId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var message = await GetOwnMessageAsync(id, messageId, currentUser.Id);
            if (message is null)
                return NotFound();

            await _chatMessageService.DeleteAsync(message);
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

        private async Task<ChatThread?> GetMemberThreadAsync(string publicId, int userId)
        {
            var thread = await _chatThreadService.GetByPublicIdAsync(publicId);
            if (thread is null || thread.IsDeleted)
                return null;

            if (thread.UserAId != userId && thread.UserBId != userId)
                return null;

            return thread;
        }

        private async Task<ChatMessage?> GetOwnMessageAsync(string threadPublicId, string messagePublicId, int userId)
        {
            var thread = await GetMemberThreadAsync(threadPublicId, userId);
            if (thread is null)
                return null;

            var message = await _chatMessageService.GetByPublicIdAsync(messagePublicId, userId);
            if (message is null || message.IsDeleted || message.ThreadId != thread.Id)
                return null;

            return message;
        }

        private async Task<ChatMessage> CreateMessageAsync(int threadId, int authorId, string text)
        {
            var message = new ChatMessage
            {
                PublicId = Guid.NewGuid().ToString(),
                ThreadId = threadId,
                AuthorId = authorId,
                Text = text
            };

            await _chatMessageService.CreateAsync(message);
            return message;
        }

        private static DateTime LatestActivity(ChatThread thread)
        {
            var latest = thread.Messages
                .Where(message => !message.IsDeleted)
                .Select(message => (DateTime?)message.CreatedAt)
                .Max();

            return latest ?? thread.CreatedAt;
        }

        private static ChatThreadDto ToThreadDto(ChatThread thread, int currentUserId)
        {
            var participant = thread.UserAId == currentUserId ? thread.UserB : thread.UserA;
            return new ChatThreadDto
            {
                Id = thread.PublicId,
                ParticipantId = participant.PublicId,
                Messages = thread.Messages
                    .Where(message => !message.IsDeleted)
                    .OrderBy(message => message.CreatedAt)
                    .Select(message => ToMessageDto(message, message.Author?.PublicId ?? string.Empty))
                    .ToList()
            };
        }

        private static ChatMessageDto ToMessageDto(ChatMessage message, string authorPublicId)
        {
            return new ChatMessageDto
            {
                Id = message.PublicId,
                AuthorId = message.Author?.PublicId ?? authorPublicId,
                Text = message.Text,
                CreatedAt = message.CreatedAt
            };
        }
    }
}
