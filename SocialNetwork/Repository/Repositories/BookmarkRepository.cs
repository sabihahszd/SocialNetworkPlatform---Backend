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
    public class BookmarkRepository : BaseRepository<Bookmark>, IBookmarkRepository
    {
        public BookmarkRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Bookmark?> FindAsync(int postId, int userId)
        {
            return await DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(bookmark => bookmark.PostId == postId && bookmark.UserId == userId);

        }
    }
}
