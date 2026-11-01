using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class SiteSettingsService : ISiteSettingsService
    {
        private readonly ISiteSettingsRepository _siteSettingsRepository;

        public SiteSettingsService(ISiteSettingsRepository siteSettingsRepository)
        {
            _siteSettingsRepository = siteSettingsRepository;
        }

        public Task CreateAsync(SiteSettings entity)
        {
            return _siteSettingsRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(SiteSettings entity)
        {
            return _siteSettingsRepository.DeleteAsync(entity);
        }

        public Task EditAsync(SiteSettings entity)
        {
            return _siteSettingsRepository.EditAsync(entity);
        }

        public Task<SiteSettings?> GetByIdAsync(int id)
        {
            return _siteSettingsRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<SiteSettings>> GetAllAsync()
        {
            return _siteSettingsRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _siteSettingsRepository.SaveAsync();
        }

        public Task<SiteSettings?> GetCurrentAsync()
        {
            return _siteSettingsRepository.GetCurrentAsync();
        }
    }
}
