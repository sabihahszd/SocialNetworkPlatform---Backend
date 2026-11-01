using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IFollowService
    {
        Task CreateAsync(Follow entity);
        Task DeleteAsync(Follow entity);
        Task EditAsync(Follow entity);
        Task<Follow?> GetByIdAsync(int id);
        Task<IEnumerable<Follow>> GetAllAsync();
        Task SaveAsync();
        Task<Follow?> FindAsync(int followerId, int followingId);
        Task<List<string>> GetFollowingPublicIdsAsync(int userId);
        Task<List<string>> GetFollowerPublicIdsAsync(int userId);
    }
}
