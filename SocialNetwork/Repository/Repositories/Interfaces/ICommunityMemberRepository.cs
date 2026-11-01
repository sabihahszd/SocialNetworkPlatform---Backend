using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface ICommunityMemberRepository : IBaseRepository<CommunityMember>
    {
        Task<CommunityMember?> FindAsync(int communityId, int userId);
        Task<bool> IsMemberAsync(int communityId, int userId);
    }
}
