using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class CommunityMemberService : ICommunityMemberService
    {
        private readonly ICommunityMemberRepository _communityMemberRepository;

        public CommunityMemberService(ICommunityMemberRepository communityMemberRepository)
        {
            _communityMemberRepository = communityMemberRepository;
        }

        public Task CreateAsync(CommunityMember entity)
        {
            return _communityMemberRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(CommunityMember entity)
        {
            return _communityMemberRepository.DeleteAsync(entity);
        }

        public Task EditAsync(CommunityMember entity)
        {
            return _communityMemberRepository.EditAsync(entity);
        }

        public Task<CommunityMember?> GetByIdAsync(int id)
        {
            return _communityMemberRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<CommunityMember>> GetAllAsync()
        {
            return _communityMemberRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _communityMemberRepository.SaveAsync();
        }

        public Task<CommunityMember?> FindAsync(int communityId, int userId)
        {
            return _communityMemberRepository.FindAsync(communityId, userId);
        }

        public Task<bool> IsMemberAsync(int communityId, int userId)
        {
            return _communityMemberRepository.IsMemberAsync(communityId, userId);
        }
    }
}
