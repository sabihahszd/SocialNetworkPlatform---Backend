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
    public class ChatMessageRepository : BaseRepository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(AppDbContext context) : base(context)
        {
        }

        public Task<ChatMessage?> GetByPublicIdAsync(string publicId, int authorId)
        {
            return DbSet.Include(message => message.Author).FirstOrDefaultAsync(message => message.PublicId == publicId && message.AuthorId == authorId);
        }

        public Task MarkReceivedAsReadAsync(int threadId, int recipientId)
        {
            var now = DateTime.UtcNow;
            return DbSet
                .Where(message => message.ThreadId == threadId && message.AuthorId != recipientId && !message.IsDeleted && !message.IsRead)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(message => message.IsRead, true)
                    .SetProperty(message => message.UpdatedAt, now));
        }
    }
}
