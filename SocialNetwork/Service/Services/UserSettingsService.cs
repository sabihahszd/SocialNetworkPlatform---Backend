using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        public UserSettingsService(IUserSettingsRepository userSettingsRepository)
        {
            _userSettingsRepository = userSettingsRepository;
        }

        public Task CreateAsync(UserSettings entity)
        {
            return _userSettingsRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(UserSettings entity)
        {
            return _userSettingsRepository.DeleteAsync(entity);
        }

        public Task EditAsync(UserSettings entity)
        {
            return _userSettingsRepository.EditAsync(entity);
        }

        public Task<UserSettings?> GetByIdAsync(int id)
        {
            return _userSettingsRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<UserSettings>> GetAllAsync()
        {
            return _userSettingsRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _userSettingsRepository.SaveAsync();
        }

        public Task<UserSettings?> GetByUserIdAsync(int userId)
        {
            return _userSettingsRepository.GetByUserIdAsync(userId);
        }
    }
}
