using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IFriendshipService
    {
        Task CreateAsync(Friendship entity);
        Task DeleteAsync(Friendship entity);
        Task EditAsync(Friendship entity);
        Task<Friendship?> GetByIdAsync(int id);
        Task<IEnumerable<Friendship>> GetAllAsync();
        Task SaveAsync();
        Task<bool> ExistsAsync(int userId, int friendId);
        Task<Friendship?> FindAsync(int userId, int friendId);
        Task<List<string>> GetFriendPublicIdsAsync(int userId);
    }
}
