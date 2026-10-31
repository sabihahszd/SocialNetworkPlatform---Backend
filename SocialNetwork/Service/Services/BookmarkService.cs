using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;

        public BookmarkService(IBookmarkRepository bookmarkRepository)
        {
            _bookmarkRepository = bookmarkRepository;
        }

        public Task CreateAsync(Bookmark entity)
        {
            return _bookmarkRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Bookmark entity)
        {
            return _bookmarkRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Bookmark entity)
        {
            return _bookmarkRepository.EditAsync(entity);
        }

        public Task<Bookmark?> GetByIdAsync(int id)
        {
            return _bookmarkRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Bookmark>> GetAllAsync()
        {
            return _bookmarkRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _bookmarkRepository.SaveAsync();
        }

        public Task<Bookmark?> FindAsync(int postId, int userId)
        {
            return _bookmarkRepository.FindAsync(postId, userId);
        }
    }
}
