using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IPostService
    {
        Task CreateAsync(Post entity);
        Task DeleteAsync(Post entity);
        Task EditAsync(Post entity);
        Task<Post?> GetByIdAsync(int id);
        Task<IEnumerable<Post>> GetAllAsync();
        Task SaveAsync();
        Task<Post?> GetByPublicIdAsync(string publicId);
        Task<Post?> GetTrackedByPublicIdAsync(string publicId);
        Task<Post?> GetDetailedByIdAsync(int id);
        Task<List<Post>> GetFeedAsync();
        Task<List<Post>> SearchAsync(string? query);
        Task<int> CountAsync();
    }
}
