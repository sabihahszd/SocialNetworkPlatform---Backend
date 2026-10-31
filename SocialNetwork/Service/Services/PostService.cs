using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public Task CreateAsync(Post entity)
        {
            return _postRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Post entity)
        {
            return _postRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Post entity)
        {
            return _postRepository.EditAsync(entity);
        }

        public Task<Post?> GetByIdAsync(int id)
        {
            return _postRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Post>> GetAllAsync()
        {
            return _postRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _postRepository.SaveAsync();
        }

        public Task<Post?> GetByPublicIdAsync(string publicId)
        {
            return _postRepository.GetByPublicIdAsync(publicId);
        }

        public Task<Post?> GetTrackedByPublicIdAsync(string publicId)
        {
            return _postRepository.GetTrackedByPublicIdAsync(publicId);
        }

        public Task<Post?> GetDetailedByIdAsync(int id)
        {
            return _postRepository.GetDetailedByIdAsync(id);
        }

        public Task<List<Post>> GetFeedAsync()
        {
            return _postRepository.GetFeedAsync();
        }

        public Task<List<Post>> SearchAsync(string? query)
        {
            return _postRepository.SearchAsync(query);
        }

        public Task<int> CountAsync()
        {
            return _postRepository.CountAsync();
        }
    }
}
