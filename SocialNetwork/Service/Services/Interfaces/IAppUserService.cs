using Domain.Entities;
using Domain.Enums;

namespace Service.Services.Interfaces
{
    public interface IAppUserService
    {
        Task CreateAsync(AppUser entity);
        Task DeleteAsync(AppUser entity);
        Task EditAsync(AppUser entity);
        Task<bool> AddToRoleAsync(AppUser user, UserRole role);
        Task<bool> RemoveFromRoleAsync(AppUser user, UserRole role);
        Task<AppUser?> GetByIdAsync(int id);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task SaveAsync();
        Task<AppUser?> GetByPublicIdAsync(string publicId);
        Task<AppUser?> FindByIdentityAsync(string identity);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<bool> UsernameTakenAsync(string username, int? exceptId = null);
        Task<bool> EmailTakenAsync(string email, int? exceptId = null);
        Task<List<AppUser>> SearchAsync(string? query);
        Task<int> CountAsync();
    }
}
