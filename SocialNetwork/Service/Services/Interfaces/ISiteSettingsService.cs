using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface ISiteSettingsService
    {
        Task CreateAsync(SiteSettings entity);
        Task DeleteAsync(SiteSettings entity);
        Task EditAsync(SiteSettings entity);
        Task<SiteSettings?> GetByIdAsync(int id);
        Task<IEnumerable<SiteSettings>> GetAllAsync();
        Task SaveAsync();
        Task<SiteSettings?> GetCurrentAsync();
    }
}
