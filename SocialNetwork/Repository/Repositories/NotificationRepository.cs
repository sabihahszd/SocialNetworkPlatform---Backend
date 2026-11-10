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
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Notification?> GetByPublicIdAsync(string publicId, int recipientId)
        {
            return DbSet.Include(item => item.Actor).FirstOrDefaultAsync(item => item.PublicId == publicId && item.RecipientId == recipientId);
        }

        public Task<List<Notification>> GetForRecipientAsync(int recipientId)
        {
            return DbSet.AsNoTracking().Include(item => item.Actor)
                           .Where(item => item.RecipientId == recipientId)
                           .OrderByDescending(item => item.CreatedAt)
                           .ThenByDescending(item => item.Id)
                           .ToListAsync();
        }

        public Task<List<Notification>> GetUnreadAsync(int recipientId)
        {
            return DbSet.Where(item => item.RecipientId == recipientId && !item.IsRead).ToListAsync();
        }
    }
}
