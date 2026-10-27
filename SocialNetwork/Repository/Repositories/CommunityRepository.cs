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
    internal class CommunityRepository : BaseRepository<Community>, ICommunityRepository
    {
        public CommunityRepository(AppDbContext context) : base(context)
        {
        }

        public Task<int> CountAsync()
        {
            return DbSet.CountAsync();
        }

        public Task<List<Community>> GetAllDetailedAsync()
        {
            return Detailed().OrderByDescending(community => community.MemberCount).ToListAsync();
        }

        public Task<Community?> GetByPublicIdAsync(string publicId)
        {
            return DbSet.AsNoTracking().Include(community => community.Owner).FirstOrDefaultAsync(community => community.PublicId == publicId);
        }

        public Task<Community?> GetDetailedByPublicIdAsync(string publicId)
        {
            return Detailed().FirstOrDefaultAsync(community => community.PublicId == publicId);
        }

        public Task<Community?> GetTrackedByPublicIdAsync(string publicId)
        {
            return DbSet.Include(community => community.Owner).FirstOrDefaultAsync(community => community.PublicId == publicId);
        }

        public Task<List<Community>> SearchAsync(string? query)
        {
            var communities = DbSet.AsNoTracking().Include(community => community.Owner).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var term = query.Trim().ToLower();
                communities = communities.Where(community => community.Name.ToLower().Contains(term) || community.Description.ToLower().Contains(term));
            }
            return communities.OrderBy(community => community.Name).ToListAsync();
        }


        private IQueryable<Community> Detailed() =>
            DbSet.AsNoTracking()
                .Include(community => community.Owner)
                .Include(community => community.Members).ThenInclude(member => member.User);
    }
}
