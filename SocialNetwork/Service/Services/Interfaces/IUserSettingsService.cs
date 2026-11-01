using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IUserSettingsService
    {
        Task CreateAsync(UserSettings entity);
        Task DeleteAsync(UserSettings entity);
        Task EditAsync(UserSettings entity);
        Task<UserSettings?> GetByIdAsync(int id);
        Task<IEnumerable<UserSettings>> GetAllAsync();
        Task SaveAsync();
        Task<UserSettings?> GetByUserIdAsync(int userId);
    }
}
