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
    public class RepostRepository : BaseRepository<Repost>, IRepostRepository
    {
        public RepostRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Repost?> FindAsync(int postId, int userId)
        {
            return DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(repost => repost.PostId == postId && repost.UserId == userId);
        }
    }
}
