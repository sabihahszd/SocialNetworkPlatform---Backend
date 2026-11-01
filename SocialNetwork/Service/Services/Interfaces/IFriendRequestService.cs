using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IFriendRequestService
    {
        Task CreateAsync(FriendRequest entity);
        Task DeleteAsync(FriendRequest entity);
        Task EditAsync(FriendRequest entity);
        Task<FriendRequest?> GetByIdAsync(int id);
        Task<IEnumerable<FriendRequest>> GetAllAsync();
        Task SaveAsync();
        Task<FriendRequest?> FindAsync(int fromUserId, int toUserId);
        Task<FriendRequest?> GetPendingAsync(int fromUserId, int toUserId);
        Task<List<string>> GetOutgoingPublicIdsAsync(int userId);
    }
}
