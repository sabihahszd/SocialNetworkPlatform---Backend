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
    public class FollowRepository : BaseRepository<Follow>, IFollowRepository
    {
        public FollowRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Follow?> FindAsync(int followerId, int followingId)
        {
            return DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(follow => follow.FollowerId == followerId && follow.FollowingId == followingId);
        }

        public Task<List<string>> GetFollowerPublicIdsAsync(int userId)
        {
            return DbSet.Where(follow => follow.FollowingId == userId).Select(follow => follow.Follower.PublicId).ToListAsync();
        }

        public Task<List<string>> GetFollowingPublicIdsAsync(int userId)
        {
            return DbSet.Where(follow => follow.FollowerId == userId).Select(follow => follow.Following.PublicId).ToListAsync();
        }
    }
}
