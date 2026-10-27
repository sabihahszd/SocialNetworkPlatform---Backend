using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface ICommunityRepository : IBaseRepository<Community>
    {
        Task<Community?> GetByPublicIdAsync(string publicId);
        Task<Community?> GetTrackedByPublicIdAsync(string publicId);
        Task<Community?> GetDetailedByPublicIdAsync(string publicId);
        Task<List<Community>> GetAllDetailedAsync();
        Task<List<Community>> SearchAsync(string? query);
        Task<int> CountAsync();
    }
}
