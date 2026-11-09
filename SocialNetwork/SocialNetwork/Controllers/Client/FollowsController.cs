using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace SocialNetwork.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
        private readonly IFollowService _followService;
        private readonly IAppUserService _userService;

        public FollowsController(IFollowService followService, IAppUserService userService)
        {
            _followService = followService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            return Ok(new
            {
                following = await _followService.GetFollowingPublicIdsAsync(currentUser.Id),
                followers = await _followService.GetFollowerPublicIdsAsync(currentUser.Id)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetForUser(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return Unauthorized();

            var user = await _userService.GetByPublicIdAsync(id);
            if (user is null || user.IsDeleted || !user.IsActive)
                return NotFound();

            return Ok(new
            {
                following = await _followService.GetFollowingPublicIdsAsync(user.Id),
                followers = await _followService.GetFollowerPublicIdsAsync(user.Id)
            });
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
    }
}
