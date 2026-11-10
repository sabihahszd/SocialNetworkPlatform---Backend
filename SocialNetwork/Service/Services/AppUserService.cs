using Domain.Entities;
using Domain.Enums;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserRepository _userRepository;

        public AppUserService(IAppUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task CreateAsync(AppUser entity)
        {
            return _userRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(AppUser entity)
        {
            return _userRepository.DeleteAsync(entity);
        }

        public Task EditAsync(AppUser entity)
        {
            return _userRepository.EditAsync(entity);
        }

        public async Task<bool> AddToRoleAsync(AppUser user, UserRole role)
        {
            if (user.Role == UserRole.SuperAdmin || role == UserRole.SuperAdmin)
                return false;

            if (user.Role == role)
                return true;

            user.Role = role;
            await EditAsync(user);
            return true;
        }

        public async Task<bool> RemoveFromRoleAsync(AppUser user, UserRole role)
        {
            if (user.Role == UserRole.SuperAdmin || user.Role != role || role == UserRole.Member)
                return false;

            user.Role = UserRole.Member;
            await EditAsync(user);
            return true;
        }

        public Task<AppUser?> GetByIdAsync(int id)
        {
            return _userRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return _userRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _userRepository.SaveAsync();
        }

        public async Task<AppUser?> GetByPublicIdAsync(string publicId)
        {
            var user = await _userRepository.GetByPublicIdAsync(publicId);
            if (user is not null)
                await ReleaseExpiredSuspensionAsync(user);
            return user;
        }

        public async Task<AppUser?> FindByIdentityAsync(string identity)
        {
            var user = await _userRepository.FindByIdentityAsync(identity);
            if (user is not null)
                await ReleaseExpiredSuspensionAsync(user);
            return user;
        }

        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is not null)
                await ReleaseExpiredSuspensionAsync(user);
            return user;
        }

        private async Task ReleaseExpiredSuspensionAsync(AppUser user)
        {
            if (user.IsBanned || user.IsDeleted || user.SuspendedUntil is not DateTime until || until > DateTime.UtcNow)
                return;

            user.IsActive = true;
            user.SuspendedUntil = null;
            await EditAsync(user);
        }

        public Task<bool> UsernameTakenAsync(string username, int? exceptId = null)
        {
            return _userRepository.UsernameTakenAsync(username, exceptId);
        }

        public Task<bool> EmailTakenAsync(string email, int? exceptId = null)
        {
            return _userRepository.EmailTakenAsync(email, exceptId);
        }

        public Task<List<AppUser>> SearchAsync(string? query)
        {
            return _userRepository.SearchAsync(query);
        }

        public Task<int> CountAsync()
        {
            return _userRepository.CountAsync();
        }
    }
}
