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
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        private readonly AppDbContext _context;

        public AppUserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<int> CountAsync()
        {
            return DbSet.CountAsync(user => !user.IsDeleted);
        }
        

        public Task<bool> EmailTakenAsync(string email, int? exceptId = null)
        {
            return DbSet.AnyAsync(user => user.Email == email && user.Id != exceptId);
        }

        public Task<AppUser?> FindByIdentityAsync(string identity)
        {
            var key = identity.Trim().TrimStart('@').ToLower();
            return DbSet.FirstOrDefaultAsync(user => user.Username == key || user.Email == key || user.Name.ToLower() == key);
        }

        public Task<AppUser?> GetByEmailAsync(string email)
        {
            return DbSet.FirstOrDefaultAsync(user => user.Email == email && !user.IsDeleted);
        }


        public Task<AppUser?> GetByPublicIdAsync(string publicId)
        {
            return DbSet.FirstOrDefaultAsync(user => user.PublicId == publicId);
        }

        public Task<List<AppUser>> SearchAsync(string? query)
        {
            var users = DbSet.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var term = query.Trim().ToLower();
                users = users.Where(user => user.Name.ToLower().Contains(term) || user.Username.ToLower().Contains(term));
            }
            return users.OrderBy(user => user.Name).ToListAsync();
        }        

        public Task<bool> UsernameTakenAsync(string username, int? exceptId = null)
        {
            var key = username.Trim().ToLower();
            return DbSet.AnyAsync(user => user.Username.ToLower() == key && user.Id != exceptId);
        }
    }
}
