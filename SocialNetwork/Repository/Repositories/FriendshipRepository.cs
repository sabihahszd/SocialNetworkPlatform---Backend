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
    public class FriendshipRepository : BaseRepository<Friendship>, IFriendshipRepository
    {
        public FriendshipRepository(AppDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(int userId, int friendId)
        {
            return DbSet.AnyAsync(friendship => friendship.UserId == userId && friendship.FriendId == friendId && !friendship.IsDeleted);
        }

        public Task<Friendship?> FindAsync(int userId, int friendId)
        {
            return DbSet.FirstOrDefaultAsync(friendship => friendship.UserId == userId && friendship.FriendId == friendId);
        }

        public Task<List<string>> GetFriendPublicIdsAsync(int userId)
        {
            return DbSet.Where(friendship => !friendship.IsDeleted && (friendship.UserId == userId || friendship.FriendId == userId))
                .Select(friendship => friendship.UserId == userId ? friendship.Friend.PublicId : friendship.User.PublicId)
                .Distinct()
                .ToListAsync();
        }
    }
}
