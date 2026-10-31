using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IRepostService
    {
        Task CreateAsync(Repost entity);
        Task DeleteAsync(Repost entity);
        Task EditAsync(Repost entity);
        Task<Repost?> GetByIdAsync(int id);
        Task<IEnumerable<Repost>> GetAllAsync();
        Task SaveAsync();
        Task<Repost?> FindAsync(int postId, int userId);
    }
}
