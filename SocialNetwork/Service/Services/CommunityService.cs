using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;

        public CommunityService(ICommunityRepository communityRepository)
        {
            _communityRepository = communityRepository;
        }

        public Task CreateAsync(Community entity)
        {
            return _communityRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Community entity)
        {
            return _communityRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Community entity)
        {
            return _communityRepository.EditAsync(entity);
        }

        public Task<Community?> GetByIdAsync(int id)
        {
            return _communityRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Community>> GetAllAsync()
        {
            return _communityRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _communityRepository.SaveAsync();
        }

        public Task<Community?> GetByPublicIdAsync(string publicId)
        {
            return _communityRepository.GetByPublicIdAsync(publicId);
        }

        public Task<Community?> GetTrackedByPublicIdAsync(string publicId)
        {
            return _communityRepository.GetTrackedByPublicIdAsync(publicId);
        }

        public Task<Community?> GetDetailedByPublicIdAsync(string publicId)
        {
            return _communityRepository.GetDetailedByPublicIdAsync(publicId);
        }

        public Task<List<Community>> GetAllDetailedAsync()
        {
            return _communityRepository.GetAllDetailedAsync();
        }

        public Task<List<Community>> SearchAsync(string? query)
        {
            return _communityRepository.SearchAsync(query);
        }

        public Task<int> CountAsync()
        {
            return _communityRepository.CountAsync();
        }
    }
}
