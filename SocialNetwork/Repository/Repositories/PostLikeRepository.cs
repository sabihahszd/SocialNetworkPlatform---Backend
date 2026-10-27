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
    public class PostLikeRepository : BaseRepository<PostLike>, IPostLikeRepository
    {
        public PostLikeRepository(AppDbContext context) : base(context)
        {
        }

        public Task<PostLike?> FindAsync(int postId, int userId)
        {
            return DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(like => like.PostId == postId && like.UserId == userId);
        }
    }
}
