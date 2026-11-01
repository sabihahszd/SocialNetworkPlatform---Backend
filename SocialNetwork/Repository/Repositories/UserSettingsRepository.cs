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
    public class UserSettingsRepository : BaseRepository<UserSettings>, IUserSettingsRepository
    {
        public UserSettingsRepository(AppDbContext context) : base(context)
        {
        }

        public Task<UserSettings?> GetByUserIdAsync(int userId)
        {
            return DbSet.FirstOrDefaultAsync(settings => settings.UserId == userId);
        }
    }
}
