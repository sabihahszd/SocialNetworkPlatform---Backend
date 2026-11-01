using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public FriendshipService(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public Task CreateAsync(Friendship entity)
        {
            return _friendshipRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Friendship entity)
        {
            return _friendshipRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Friendship entity)
        {
            return _friendshipRepository.EditAsync(entity);
        }

        public Task<Friendship?> GetByIdAsync(int id)
        {
            return _friendshipRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Friendship>> GetAllAsync()
        {
            return _friendshipRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _friendshipRepository.SaveAsync();
        }

        public Task<bool> ExistsAsync(int userId, int friendId)
        {
            return _friendshipRepository.ExistsAsync(userId, friendId);
        }

        public Task<List<string>> GetFriendPublicIdsAsync(int userId)
        {
            return _friendshipRepository.GetFriendPublicIdsAsync(userId);
        }
    }
}
