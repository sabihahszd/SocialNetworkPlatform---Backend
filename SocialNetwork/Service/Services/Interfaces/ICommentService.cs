using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface ICommentService
    {
        Task CreateAsync(Comment entity);
        Task DeleteAsync(Comment entity);
        Task EditAsync(Comment entity);
        Task<Comment?> GetByIdAsync(int id);
        Task<IEnumerable<Comment>> GetAllAsync();
        Task SaveAsync();
        Task<Comment?> GetByPublicIdAsync(string publicId);
        Task<Comment?> GetTrackedByPublicIdAsync(string publicId);
    }
}
