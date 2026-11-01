using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepository;

        public FollowService(IFollowRepository followRepository)
        {
            _followRepository = followRepository;
        }

        public Task CreateAsync(Follow entity)
        {
            return _followRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Follow entity)
        {
            return _followRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Follow entity)
        {
            return _followRepository.EditAsync(entity);
        }

        public Task<Follow?> GetByIdAsync(int id)
        {
            return _followRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Follow>> GetAllAsync()
        {
            return _followRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _followRepository.SaveAsync();
        }

        public Task<Follow?> FindAsync(int followerId, int followingId)
        {
            return _followRepository.FindAsync(followerId, followingId);
        }

        public Task<List<string>> GetFollowingPublicIdsAsync(int userId)
        {
            return _followRepository.GetFollowingPublicIdsAsync(userId);
        }

        public Task<List<string>> GetFollowerPublicIdsAsync(int userId)
        {
            return _followRepository.GetFollowerPublicIdsAsync(userId);
        }
    }
}
