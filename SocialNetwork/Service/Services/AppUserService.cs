using Domain.Entities;
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

        public Task<AppUser?> GetByPublicIdAsync(string publicId)
        {
            return _userRepository.GetByPublicIdAsync(publicId);
        }

        public Task<AppUser?> FindByIdentityAsync(string identity)
        {
            return _userRepository.FindByIdentityAsync(identity);
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
