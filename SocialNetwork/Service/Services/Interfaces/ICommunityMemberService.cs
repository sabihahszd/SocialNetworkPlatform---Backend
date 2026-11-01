using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface ICommunityMemberService
    {
        Task CreateAsync(CommunityMember entity);
        Task DeleteAsync(CommunityMember entity);
        Task EditAsync(CommunityMember entity);
        Task<CommunityMember?> GetByIdAsync(int id);
        Task<IEnumerable<CommunityMember>> GetAllAsync();
        Task SaveAsync();
        Task<CommunityMember?> FindAsync(int communityId, int userId);
        Task<bool> IsMemberAsync(int communityId, int userId);
    }
}
