using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IAppUserRepository : IBaseRepository<AppUser>
    {
        Task<AppUser?> GetByPublicIdAsync(string publicId);
        Task<AppUser?> FindByIdentityAsync(string identity);
        Task<bool> UsernameTakenAsync(string username, int? exceptId = null);
        Task<bool> EmailTakenAsync(string email, int? exceptId = null);
        Task<List<AppUser>> SearchAsync(string? query);
        Task<int> CountAsync();
    }
}
