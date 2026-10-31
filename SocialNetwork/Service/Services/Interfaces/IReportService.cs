using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IReportService
    {
        Task CreateAsync(Report entity);
        Task DeleteAsync(Report entity);
        Task EditAsync(Report entity);
        Task<Report?> GetByIdAsync(int id);
        Task<IEnumerable<Report>> GetAllAsync();
        Task SaveAsync();
        Task<Report?> GetByPublicIdAsync(string publicId);
        Task<Report?> GetOpenByPublicIdAsync(string publicId);
        Task<List<Report>> GetOpenAsync(int? reporterId = null);
        Task<List<Report>> GetOpenByPostIdAsync(int postId);
        Task<bool> HasOpenAsync(int postId, int reporterId);
        Task<int> CountOpenAsync();
    }
}
