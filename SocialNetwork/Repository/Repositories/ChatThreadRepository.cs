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
    public class ChatThreadRepository : BaseRepository<ChatThread>, IChatThreadRepository
    {
        public ChatThreadRepository(AppDbContext context) : base(context)
        {
        }

        public Task<ChatThread?> GetBetweenAsync(int userId, int otherId)
        {
           return DbSet.Include(thread => thread.UserA).Include(thread => thread.UserB)
                           .FirstOrDefaultAsync(thread =>
                               (thread.UserAId == userId && thread.UserBId == otherId) ||
                               (thread.UserAId == otherId && thread.UserBId == userId));
        }

        public Task<ChatThread?> GetByPublicIdAsync(string publicId)
        {
           return DbSet.Include(thread => thread.UserA).Include(thread => thread.UserB)
                            .FirstOrDefaultAsync(thread => thread.PublicId == publicId);
        }

        public Task<List<ChatThread>> GetForUserAsync(int userId)
        {
           return DbSet.AsNoTracking()
                           .Include(thread => thread.UserA)
                           .Include(thread => thread.UserB)
                           .Include(thread => thread.Messages).ThenInclude(message => message.Author)
                           .Where(thread => thread.UserAId == userId || thread.UserBId == userId)
                           .ToListAsync();
        }

    }
}
