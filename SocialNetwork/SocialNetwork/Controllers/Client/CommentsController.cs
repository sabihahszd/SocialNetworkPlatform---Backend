using System.Security.Claims;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.DTOs.Comment;
using Service.Helpers.DTOs.Common;
using Service.Helpers.Validators;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private const int MaxDepth = 3;

        private readonly ICommentService _commentService;
        private readonly IPostService _postService;
        private readonly IAppUserService _userService;
        private readonly IFriendshipService _friendshipService;
        private readonly ICommunityMemberService _communityMemberService;
        private readonly IValidator<CommentRequestDto> _commentValidator;
        private readonly IValidator<UpdateTextRequest> _textValidator;

        public CommentsController(
            ICommentService commentService,
            IPostService postService,
            IAppUserService userService,
            IFriendshipService friendshipService,
            ICommunityMemberService communityMemberService,
            IValidator<CommentRequestDto> commentValidator,
            IValidator<UpdateTextRequest> textValidator)
        {
            _commentService = commentService;
            _postService = postService;
            _userService = userService;
            _friendshipService = friendshipService;
            _communityMemberService = communityMemberService;
            _commentValidator = commentValidator;
            _textValidator = textValidator;
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetByPost(string postId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var post = await GetVisiblePostAsync(postId, currentUser);
            if (post is null)
                return NotFound();

            return Ok(ToTree(post.Comments));
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> Create(string postId, [FromBody] CommentRequestDto request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var error = await _commentValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var text = request.Text.Trim();

            var post = await GetVisiblePostAsync(postId, currentUser);
            if (post is null)
                return NotFound();

            int? parentId = null;
            if (!string.IsNullOrWhiteSpace(request.ParentId))
            {
                var parent = await _commentService.GetByPublicIdAsync(request.ParentId.Trim());
                if (parent is null || parent.IsDeleted || parent.PostId != post.Id)
                    return NotFound();

                if (!await WithinReplyDepthAsync(parent))
                    return BadRequest(new { message = "Replies can only be nested three levels deep." });

                parentId = parent.Id;
            }

            var comment = new Comment
            {
                PublicId = Guid.NewGuid().ToString(),
                PostId = post.Id,
                AuthorId = currentUser.Id,
                ParentId = parentId,
                Text = text
            };

            await _commentService.CreateAsync(comment);
            return Ok(ToDto(comment, currentUser.PublicId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] UpdateTextRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var error = await _textValidator.FirstErrorAsync(request, UpdateTextRequestValidator.Comment);
            if (error is not null)
                return BadRequest(new { message = error });

            var text = request.Text.Trim();

            var comment = await GetOwnCommentAsync(id, currentUser.Id);
            if (comment is null)
                return NotFound();

            comment.Text = text;
            await _commentService.EditAsync(comment);
            return Ok(ToDto(comment, currentUser.PublicId));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var comment = await GetOwnCommentAsync(id, currentUser.Id);
            if (comment is null)
                return NotFound();

            await _commentService.DeleteAsync(comment);
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

        private async Task<Post?> GetVisiblePostAsync(string publicId, AppUser currentUser)
        {
            var post = await _postService.GetByPublicIdAsync(publicId);
            if (post is null || post.IsDeleted)
                return null;

            var detailed = await _postService.GetDetailedByIdAsync(post.Id);
            if (detailed is null || detailed.IsDeleted || !await CanSeeAsync(detailed, currentUser))
                return null;

            return detailed;
        }

        private async Task<bool> CanSeeAsync(Post post, AppUser currentUser)
        {
            if (post.Author is { IsActive: false, IsDeleted: false } || post.Author is { IsDeleted: true })
                return false;

            if (post.AuthorId == currentUser.Id || post.Privacy == PostPrivacy.Public)
                return true;

            if (post.Privacy == PostPrivacy.Friends)
            {
                return await _friendshipService.ExistsAsync(currentUser.Id, post.AuthorId)
                    || await _friendshipService.ExistsAsync(post.AuthorId, currentUser.Id);
            }

            if (post.Privacy != PostPrivacy.Communities)
                return false;

            var communityIds = post.PostCommunities
                .Where(link => !link.IsDeleted)
                .Select(link => link.CommunityId)
                .ToHashSet();

            if (post.CommunityId is int communityId)
                communityIds.Add(communityId);

            foreach (var id in communityIds)
            {
                if (await _communityMemberService.IsMemberAsync(id, currentUser.Id))
                    return true;
            }

            return false;
        }

        private async Task<bool> WithinReplyDepthAsync(Comment parent)
        {
            var depth = 1;
            var parentId = parent.ParentId;
            while (parentId is int id)
            {
                depth++;
                if (depth >= MaxDepth)
                    return false;

                var ancestor = await _commentService.GetByIdAsync(id);
                if (ancestor is null || ancestor.IsDeleted)
                    return false;

                parentId = ancestor.ParentId;
            }

            return true;
        }

        private async Task<Comment?> GetOwnCommentAsync(string publicId, int userId)
        {
            var comment = await _commentService.GetTrackedByPublicIdAsync(publicId);
            if (comment is null || comment.IsDeleted || comment.AuthorId != userId)
                return null;

            var post = await _postService.GetByIdAsync(comment.PostId);
            if (post is null || post.IsDeleted)
                return null;

            return comment;
        }

        private static List<CommentDto> ToTree(IEnumerable<Comment> comments)
        {
            var visible = comments
                .Where(comment => !comment.IsDeleted)
                .GroupBy(comment => comment.Id)
                .Select(group => group.First())
                .ToList();

            var byParent = visible.ToLookup(comment => comment.ParentId);

            List<CommentDto> Map(int? parentId)
            {
                return byParent[parentId]
                    .OrderBy(comment => comment.CreatedAt)
                    .Select(comment =>
                    {
                        var dto = ToDto(comment, comment.Author?.PublicId ?? string.Empty);
                        dto.Replies = Map(comment.Id);
                        return dto;
                    })
                    .ToList();
            }

            return Map(null);
        }

        private static CommentDto ToDto(Comment comment, string authorPublicId)
        {
            return new CommentDto
            {
                Id = comment.PublicId,
                AuthorId = comment.Author?.PublicId ?? authorPublicId,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}
