using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IBookmarkService
    {
        Task CreateAsync(Bookmark entity);
        Task DeleteAsync(Bookmark entity);
        Task EditAsync(Bookmark entity);
        Task<Bookmark?> GetByIdAsync(int id);
        Task<IEnumerable<Bookmark>> GetAllAsync();
        Task SaveAsync();
        Task<Bookmark?> FindAsync(int postId, int userId);
    }
}
