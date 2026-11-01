using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly IFriendRequestRepository _friendRequestRepository;

        public FriendRequestService(IFriendRequestRepository friendRequestRepository)
        {
            _friendRequestRepository = friendRequestRepository;
        }

        public Task CreateAsync(FriendRequest entity)
        {
            return _friendRequestRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(FriendRequest entity)
        {
            return _friendRequestRepository.DeleteAsync(entity);
        }

        public Task EditAsync(FriendRequest entity)
        {
            return _friendRequestRepository.EditAsync(entity);
        }

        public Task<FriendRequest?> GetByIdAsync(int id)
        {
            return _friendRequestRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<FriendRequest>> GetAllAsync()
        {
            return _friendRequestRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _friendRequestRepository.SaveAsync();
        }

        public Task<FriendRequest?> FindAsync(int fromUserId, int toUserId)
        {
            return _friendRequestRepository.FindAsync(fromUserId, toUserId);
        }

        public Task<FriendRequest?> GetPendingAsync(int fromUserId, int toUserId)
        {
            return _friendRequestRepository.GetPendingAsync(fromUserId, toUserId);
        }

        public Task<List<string>> GetOutgoingPublicIdsAsync(int userId)
        {
            return _friendRequestRepository.GetOutgoingPublicIdsAsync(userId);
        }
    }
}
