using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class CommunityMemberRepository : BaseRepository<CommunityMember>, ICommunityMemberRepository
    {
        public CommunityMemberRepository(AppDbContext context) : base(context)
        {
        }

        public Task<CommunityMember?> FindAsync(int communityId, int userId)
        {
            return DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(member => member.CommunityId == communityId && member.UserId == userId);
        }

        public Task<bool> IsMemberAsync(int communityId, int userId)
        {
            return DbSet.AnyAsync(member => member.CommunityId == communityId && member.UserId == userId);
        }
    }
}
