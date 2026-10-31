using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface ICommunityService
    {
        Task CreateAsync(Community entity);
        Task DeleteAsync(Community entity);
        Task EditAsync(Community entity);
        Task<Community?> GetByIdAsync(int id);
        Task<IEnumerable<Community>> GetAllAsync();
        Task SaveAsync();
        Task<Community?> GetByPublicIdAsync(string publicId);
        Task<Community?> GetTrackedByPublicIdAsync(string publicId);
        Task<Community?> GetDetailedByPublicIdAsync(string publicId);
        Task<List<Community>> GetAllDetailedAsync();
        Task<List<Community>> SearchAsync(string? query);
        Task<int> CountAsync();
    }
}
