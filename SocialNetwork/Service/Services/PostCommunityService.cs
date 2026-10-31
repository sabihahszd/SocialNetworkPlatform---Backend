using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class PostCommunityService : IPostCommunityService
    {
        private readonly IPostCommunityRepository _postCommunityRepository;

        public PostCommunityService(IPostCommunityRepository postCommunityRepository)
        {
            _postCommunityRepository = postCommunityRepository;
        }

        public Task CreateAsync(PostCommunity entity)
        {
            return _postCommunityRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(PostCommunity entity)
        {
            return _postCommunityRepository.DeleteAsync(entity);
        }

        public Task EditAsync(PostCommunity entity)
        {
            return _postCommunityRepository.EditAsync(entity);
        }

        public Task<PostCommunity?> GetByIdAsync(int id)
        {
            return _postCommunityRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<PostCommunity>> GetAllAsync()
        {
            return _postCommunityRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _postCommunityRepository.SaveAsync();
        }

        public Task<bool> ExistsAsync(int postId, int communityId)
        {
            return _postCommunityRepository.ExistsAsync(postId, communityId);
        }
    }
}
