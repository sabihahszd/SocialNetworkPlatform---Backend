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
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Comment?> GetByPublicIdAsync(string publicId)
        {
            return DbSet.AsNoTracking().Include(comment => comment.Author).FirstOrDefaultAsync(comment => comment.PublicId == publicId);
        }

        public Task<Comment?> GetTrackedByPublicIdAsync(string publicId)
        {
            return DbSet.FirstOrDefaultAsync(comment => comment.PublicId == publicId);
        }
    }
}
