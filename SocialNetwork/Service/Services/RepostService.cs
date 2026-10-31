using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class RepostService : IRepostService
    {
        private readonly IRepostRepository _repostRepository;

        public RepostService(IRepostRepository repostRepository)
        {
            _repostRepository = repostRepository;
        }

        public Task CreateAsync(Repost entity)
        {
            return _repostRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Repost entity)
        {
            return _repostRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Repost entity)
        {
            return _repostRepository.EditAsync(entity);
        }

        public Task<Repost?> GetByIdAsync(int id)
        {
            return _repostRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Repost>> GetAllAsync()
        {
            return _repostRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _repostRepository.SaveAsync();
        }

        public Task<Repost?> FindAsync(int postId, int userId)
        {
            return _repostRepository.FindAsync(postId, userId);
        }
    }
}
