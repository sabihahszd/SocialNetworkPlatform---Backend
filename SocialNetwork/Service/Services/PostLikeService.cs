using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class PostLikeService : IPostLikeService
    {
        private readonly IPostLikeRepository _postLikeRepository;

        public PostLikeService(IPostLikeRepository postLikeRepository)
        {
            _postLikeRepository = postLikeRepository;
        }

        public Task CreateAsync(PostLike entity)
        {
            return _postLikeRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(PostLike entity)
        {
            return _postLikeRepository.DeleteAsync(entity);
        }

        public Task EditAsync(PostLike entity)
        {
            return _postLikeRepository.EditAsync(entity);
        }

        public Task<PostLike?> GetByIdAsync(int id)
        {
            return _postLikeRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<PostLike>> GetAllAsync()
        {
            return _postLikeRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _postLikeRepository.SaveAsync();
        }

        public Task<PostLike?> FindAsync(int postId, int userId)
        {
            return _postLikeRepository.FindAsync(postId, userId);
        }
    }
}
