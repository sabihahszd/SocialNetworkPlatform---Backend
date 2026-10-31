using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IPostCommunityService
    {
        Task CreateAsync(PostCommunity entity);
        Task DeleteAsync(PostCommunity entity);
        Task EditAsync(PostCommunity entity);
        Task<PostCommunity?> GetByIdAsync(int id);
        Task<IEnumerable<PostCommunity>> GetAllAsync();
        Task SaveAsync();
        Task<bool> ExistsAsync(int postId, int communityId);
    }
}
