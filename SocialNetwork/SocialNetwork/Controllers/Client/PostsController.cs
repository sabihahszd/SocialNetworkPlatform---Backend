using System.Security.Claims;
using System.Text.Json;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Helpers.DTOs.Comment;
using Service.Helpers.DTOs.Common;
using Service.Helpers.DTOs.Post;
using Service.Helpers.Validators;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IPostLikeService _postLikeService;
        private readonly IRepostService _repostService;
        private readonly IBookmarkService _bookmarkService;
        private readonly IPostCommunityService _postCommunityService;
        private readonly ICommunityService _communityService;
        private readonly ICommunityMemberService _communityMemberService;
        private readonly IFriendshipService _friendshipService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly INotificationService _notificationService;
        private readonly IAppUserService _userService;
        private readonly IValidator<CreatePostRequest> _createPostValidator;
        private readonly IValidator<UpdateTextRequest> _textValidator;

        public PostsController(
            IPostService postService,
            IPostLikeService postLikeService,
            IRepostService repostService,
            IBookmarkService bookmarkService,
            IPostCommunityService postCommunityService,
            ICommunityService communityService,
            ICommunityMemberService communityMemberService,
            IFriendshipService friendshipService,
            IUserSettingsService userSettingsService,
            INotificationService notificationService,
            IAppUserService userService,
            IValidator<CreatePostRequest> createPostValidator,
            IValidator<UpdateTextRequest> textValidator)
        {
            _postService = postService;
            _postLikeService = postLikeService;
            _repostService = repostService;
            _bookmarkService = bookmarkService;
            _postCommunityService = postCommunityService;
            _communityService = communityService;
            _communityMemberService = communityMemberService;
            _friendshipService = friendshipService;
            _userSettingsService = userSettingsService;
            _notificationService = notificationService;
            _userService = userService;
            _createPostValidator = createPostValidator;
            _textValidator = textValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeed([FromQuery] string? feed)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var posts = await _postService.GetFeedAsync();
            return Ok(await VisiblePostsAsync(posts, currentUser, feed));
        }

        [HttpGet("bookmarks")]
        public async Task<IActionResult> GetBookmarks()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var posts = await _postService.GetFeedAsync();
            var saved = posts.Where(post => post.Bookmarks.Any(bookmark => !bookmark.IsDeleted && bookmark.UserId == currentUser.Id));
            return Ok(await VisiblePostsAsync(saved, currentUser, null));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var matches = await _postService.SearchAsync(q);
            var detailed = new List<Post>();
            foreach (var match in matches.Where(post => !post.IsDeleted))
            {
                var post = await _postService.GetDetailedByIdAsync(match.Id);
                if (post is not null)
                    detailed.Add(post);
            }

            return Ok(await VisiblePostsAsync(detailed, currentUser, null));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var post = await GetVisiblePostAsync(id, currentUser);
            if (post is null)
                return NotFound();

            return Ok(ToDto(post));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var error = await _createPostValidator.FirstErrorAsync(request);
            if (error is not null)
                return BadRequest(new { message = error });

            var text = request.Text.Trim();
            var privacy = ParsePrivacy(request.Privacy);

            var communitiesResult = await ResolveCommunitiesAsync(request, currentUser.Id);
            if (communitiesResult.Error is not null)
                return communitiesResult.Error;

            var communities = communitiesResult.Communities;
            if (privacy == PostPrivacy.Communities && communities.Count == 0)
                return BadRequest(new { message = "Choose at least one community." });

            var post = new Post
            {
                PublicId = Guid.NewGuid().ToString(),
                AuthorId = currentUser.Id,
                Text = text,
                ImageUrl = request.Image?.Trim() ?? string.Empty,
                VideoUrl = request.Video?.Trim() ?? string.Empty,
                Privacy = privacy,
                CommunityId = communities.FirstOrDefault()?.Id
            };

            await _postService.CreateAsync(post);

            foreach (var community in communities)
            {
                if (await _postCommunityService.ExistsAsync(post.Id, community.Id))
                    continue;

                try
                {
                    await _postCommunityService.CreateAsync(new PostCommunity
                    {
                        PostId = post.Id,
                        CommunityId = community.Id
                    });
                }
                catch (DbUpdateException)
                {
                }
            }

            var detailed = await _postService.GetDetailedByIdAsync(post.Id);
            return Ok(ToDto(detailed ?? post));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] UpdateTextRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var error = await _textValidator.FirstErrorAsync(request, UpdateTextRequestValidator.Post);
            if (error is not null)
                return BadRequest(new { message = error });

            var text = request.Text.Trim();

            var post = await GetOwnPostAsync(id, currentUser.Id);
            if (post is null)
                return NotFound();

            post.Text = text;
            await _postService.EditAsync(post);

            var detailed = await _postService.GetDetailedByIdAsync(post.Id);
            return Ok(ToDto(detailed ?? post));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var post = await GetOwnPostAsync(id, currentUser.Id);
            if (post is null)
                return NotFound();

            await _postService.DeleteAsync(post);
            return NoContent();
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> ToggleLike(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var post = await GetVisiblePostAsync(id, currentUser);
            if (post is null)
                return NotFound();

            var added = await ToggleLikeAsync(post.Id, currentUser.Id);
            if (added)
                await NotifyAsync(post, currentUser, NotificationType.Like, settings => settings?.NotifyLikes ?? true);

            var detailed = await _postService.GetDetailedByIdAsync(post.Id);
            return Ok(ToDto(detailed ?? post));
        }

        [HttpPost("{id}/repost")]
        public async Task<IActionResult> ToggleRepost(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var post = await GetVisiblePostAsync(id, currentUser);
            if (post is null)
                return NotFound();

            var added = await ToggleRepostAsync(post.Id, currentUser.Id);
            if (added)
                await NotifyAsync(post, currentUser, NotificationType.Repost, settings => settings?.NotifyReposts ?? true);

            var detailed = await _postService.GetDetailedByIdAsync(post.Id);
            return Ok(ToDto(detailed ?? post));
        }

        [HttpPost("{id}/bookmark")]
        public async Task<IActionResult> ToggleBookmark(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var post = await GetVisiblePostAsync(id, currentUser);
            if (post is null)
                return NotFound();

            await ToggleBookmarkAsync(post.Id, currentUser.Id);
            var detailed = await _postService.GetDetailedByIdAsync(post.Id);
            return Ok(ToDto(detailed ?? post));
        }

        private async Task<List<PostDto>> VisiblePostsAsync(IEnumerable<Post> posts, AppUser currentUser, string? feed)
        {
            var friendsOnly = string.Equals(feed, "friends", StringComparison.OrdinalIgnoreCase);
            var blockedIds = await GetBlockedIdsAsync(currentUser.Id);
            var friendIds = (await _friendshipService.GetFriendPublicIdsAsync(currentUser.Id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var membership = new Dictionary<int, bool>();
            var results = new List<PostDto>();

            foreach (var post in posts)
            {
                if (!await CanSeeAsync(post, currentUser, blockedIds, friendIds, membership))
                    continue;

                if (friendsOnly && post.AuthorId != currentUser.Id && !await IsFriendAsync(post, currentUser.Id, friendIds))
                    continue;

                results.Add(ToDto(post));
            }

            return results;
        }

        private async Task<Post?> GetVisiblePostAsync(string publicId, AppUser currentUser)
        {
            var post = await _postService.GetByPublicIdAsync(publicId);
            if (post is null || post.IsDeleted)
                return null;

            var detailed = await _postService.GetDetailedByIdAsync(post.Id);
            if (detailed is null || detailed.IsDeleted)
                return null;

            var blockedIds = await GetBlockedIdsAsync(currentUser.Id);
            var friendIds = (await _friendshipService.GetFriendPublicIdsAsync(currentUser.Id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!await CanSeeAsync(detailed, currentUser, blockedIds, friendIds, new Dictionary<int, bool>()))
                return null;

            return detailed;
        }

        private async Task<bool> CanSeeAsync(
            Post post,
            AppUser currentUser,
            IReadOnlySet<string> blockedIds,
            IReadOnlySet<string> friendIds,
            Dictionary<int, bool> membership)
        {
            if (post.IsDeleted)
                return false;

            if (post.Author is { IsActive: false } or { IsDeleted: true })
                return false;

            if (post.AuthorId != currentUser.Id
                && post.Author is not null
                && blockedIds.Contains(post.Author.PublicId))
                return false;

            if (post.AuthorId == currentUser.Id || post.Privacy == PostPrivacy.Public)
                return true;

            if (post.Privacy == PostPrivacy.Friends)
                return await IsFriendAsync(post, currentUser.Id, friendIds);

            if (post.Privacy != PostPrivacy.Communities)
                return false;

            foreach (var communityId in CommunityIds(post))
            {
                if (!membership.TryGetValue(communityId, out var isMember))
                {
                    isMember = await _communityMemberService.IsMemberAsync(communityId, currentUser.Id);
                    membership[communityId] = isMember;
                }

                if (isMember)
                    return true;
            }

            return false;
        }

        private async Task<bool> IsFriendAsync(Post post, int currentUserId, IReadOnlySet<string> friendIds)
        {
            if (post.Author is not null && friendIds.Contains(post.Author.PublicId))
                return true;

            return await _friendshipService.ExistsAsync(post.AuthorId, currentUserId);
        }

        private async Task<Post?> GetOwnPostAsync(string publicId, int userId)
        {
            var post = await _postService.GetTrackedByPublicIdAsync(publicId);
            if (post is null || post.IsDeleted || post.AuthorId != userId)
                return null;

            return post;
        }

        private async Task<(List<Community> Communities, IActionResult? Error)> ResolveCommunitiesAsync(CreatePostRequest request, int userId)
        {
            var publicIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(request.CommunityId))
                publicIds.Add(request.CommunityId.Trim());

            if (request.CommunityIds is not null)
                publicIds.AddRange(request.CommunityIds.Where(id => !string.IsNullOrWhiteSpace(id)).Select(id => id.Trim()));

            var communities = new List<Community>();
            foreach (var publicId in publicIds.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var community = await _communityService.GetByPublicIdAsync(publicId);
                if (community is null || community.IsDeleted)
                    return (communities, NotFound());

                if (!await _communityMemberService.IsMemberAsync(community.Id, userId))
                    return (communities, BadRequest(new { message = "You can only post in a community you belong to." }));

                communities.Add(community);
            }

            return (communities, null);
        }

        private async Task<bool> ToggleLikeAsync(int postId, int userId)
        {
            var existing = await _postLikeService.FindAsync(postId, userId);
            if (existing is null)
            {
                try
                {
                    await _postLikeService.CreateAsync(new PostLike { PostId = postId, UserId = userId });
                }
                catch (DbUpdateException)
                {
                }

                return true;
            }

            if (existing.IsDeleted)
            {
                existing.IsDeleted = false;
                await _postLikeService.EditAsync(existing);
                return true;
            }

            await _postLikeService.DeleteAsync(existing);
            return false;
        }

        private async Task<bool> ToggleRepostAsync(int postId, int userId)
        {
            var existing = await _repostService.FindAsync(postId, userId);
            if (existing is null)
            {
                try
                {
                    await _repostService.CreateAsync(new Repost { PostId = postId, UserId = userId });
                }
                catch (DbUpdateException)
                {
                }

                return true;
            }

            if (existing.IsDeleted)
            {
                existing.IsDeleted = false;
                await _repostService.EditAsync(existing);
                return true;
            }

            await _repostService.DeleteAsync(existing);
            return false;
        }

        private async Task ToggleBookmarkAsync(int postId, int userId)
        {
            var existing = await _bookmarkService.FindAsync(postId, userId);
            if (existing is null)
            {
                try
                {
                    await _bookmarkService.CreateAsync(new Bookmark { PostId = postId, UserId = userId });
                }
                catch (DbUpdateException)
                {
                }

                return;
            }

            if (existing.IsDeleted)
            {
                existing.IsDeleted = false;
                await _bookmarkService.EditAsync(existing);
                return;
            }

            await _bookmarkService.DeleteAsync(existing);
        }

        private async Task NotifyAsync(Post post, AppUser actor, NotificationType type, Func<UserSettings?, bool> enabled)
        {
            if (post.AuthorId == actor.Id)
                return;

            var settings = await _userSettingsService.GetByUserIdAsync(post.AuthorId);
            if (!enabled(settings))
                return;

            var detail = post.Text.Trim();
            if (detail.Length > 180)
                detail = detail[..180];

            await _notificationService.CreateAsync(new Notification
            {
                PublicId = Guid.NewGuid().ToString(),
                RecipientId = post.AuthorId,
                ActorId = actor.Id,
                Type = type,
                PostPublicId = post.PublicId,
                Detail = detail,
                IsRead = false,
                Status = RequestStatus.Done
            });
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

        private static IEnumerable<int> CommunityIds(Post post)
        {
            var ids = post.PostCommunities
                .Where(link => !link.IsDeleted)
                .Select(link => link.CommunityId)
                .ToHashSet();

            if (post.CommunityId is int communityId)
                ids.Add(communityId);

            return ids;
        }

        private static PostPrivacy ParsePrivacy(string? value)
        {
            return value?.Trim().ToLowerInvariant() switch
            {
                "friends" => PostPrivacy.Friends,
                "communities" => PostPrivacy.Communities,
                _ => PostPrivacy.Public
            };
        }

        private static PostDto ToDto(Post post)
        {
            var communityIds = post.PostCommunities
                .Where(link => !link.IsDeleted && link.Community is not null)
                .Select(link => link.Community.PublicId)
                .Distinct()
                .ToList();

            if (post.Community is not null && !communityIds.Contains(post.Community.PublicId))
                communityIds.Insert(0, post.Community.PublicId);

            return new PostDto
            {
                Id = post.PublicId,
                AuthorId = post.Author?.PublicId ?? string.Empty,
                CommunityId = post.Community?.PublicId,
                CommunityIds = communityIds,
                Text = post.Text,
                Image = post.ImageUrl,
                Video = post.VideoUrl,
                Privacy = ToPrivacy(post.Privacy),
                CreatedAt = post.CreatedAt,
                Likes = PublicIds(post.Likes),
                Reposts = PublicIds(post.Reposts),
                Bookmarks = PublicIds(post.Bookmarks),
                Comments = ToCommentTree(post.Comments)
            };
        }

        private static List<string> PublicIds<T>(IEnumerable<T> items) where T : BaseEntity
        {
            return items
                .Where(item => !item.IsDeleted)
                .Select(item => item switch
                {
                    PostLike like => like.User?.PublicId,
                    Repost repost => repost.User?.PublicId,
                    Bookmark bookmark => bookmark.User?.PublicId,
                    _ => null
                })
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id!)
                .Distinct()
                .ToList();
        }

        private static List<CommentDto> ToCommentTree(IEnumerable<Comment> comments)
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
                        var dto = new CommentDto
                        {
                            Id = comment.PublicId,
                            AuthorId = comment.Author?.PublicId ?? string.Empty,
                            Text = comment.Text,
                            CreatedAt = comment.CreatedAt
                        };
                        dto.Replies = Map(comment.Id);
                        return dto;
                    })
                    .ToList();
            }

            return Map(null);
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
    }
}
