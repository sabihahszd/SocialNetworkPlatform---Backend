using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IAppUserService
    {
        Task CreateAsync(AppUser entity);
        Task DeleteAsync(AppUser entity);
        Task EditAsync(AppUser entity);
        Task<AppUser?> GetByIdAsync(int id);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task SaveAsync();
        Task<AppUser?> GetByPublicIdAsync(string publicId);
        Task<AppUser?> FindByIdentityAsync(string identity);
        Task<bool> UsernameTakenAsync(string username, int? exceptId = null);
        Task<bool> EmailTakenAsync(string email, int? exceptId = null);
        Task<List<AppUser>> SearchAsync(string? query);
        Task<int> CountAsync();
    }
}
