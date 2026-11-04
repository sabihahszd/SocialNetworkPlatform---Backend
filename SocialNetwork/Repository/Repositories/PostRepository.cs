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
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        private readonly AppDbContext _context;
        public PostRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<int> CountAsync()
        {
            return DbSet.CountAsync(post => !post.IsDeleted);
        }

        public Task<Post?> GetByPublicIdAsync(string publicId)
        {
            return DbSet.AsNoTracking().Include(post => post.Author).FirstOrDefaultAsync(post => post.PublicId == publicId);
        }

        public Task<Post?> GetDetailedByIdAsync(int id)
        {
            return Detailed().FirstOrDefaultAsync(post => post.Id == id);
        }

        public Task<List<Post>> GetFeedAsync()
        {
            return Detailed().OrderByDescending(post => post.CreatedAt).ToListAsync();
        }

        public Task<Post?> GetTrackedByPublicIdAsync(string publicId)
        {
            return DbSet.Include(post => post.Author).FirstOrDefaultAsync(post => post.PublicId == publicId);
        }

        public Task<List<Post>> SearchAsync(string? query)
        {
            var posts = DbSet.AsNoTracking().Include(post => post.Author).Include(post => post.Community).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var term = query.Trim().ToLower();
                posts = posts.Where(post => post.Text.ToLower().Contains(term) || post.Author.Name.ToLower().Contains(term) || post.Author.Username.ToLower().Contains(term));
            }
            return posts.OrderByDescending(post => post.CreatedAt).ToListAsync();
        }        


        private IQueryable<Post> Detailed() =>
            DbSet.AsNoTracking()
                .Include(post => post.Author)
                .Include(post => post.Community)
                .Include(post => post.PostCommunities).ThenInclude(link => link.Community)
                .Include(post => post.Likes).ThenInclude(like => like.User)
                .Include(post => post.Reposts).ThenInclude(repost => repost.User)
                .Include(post => post.Bookmarks).ThenInclude(bookmark => bookmark.User)
                .Include(post => post.Comments).ThenInclude(comment => comment.Author)
                .Include(post => post.Comments).ThenInclude(comment => comment.Replies).ThenInclude(reply => reply.Author)
                .Include(post => post.Comments).ThenInclude(comment => comment.Replies).ThenInclude(reply => reply.Replies).ThenInclude(reply => reply.Author);
    }

}

