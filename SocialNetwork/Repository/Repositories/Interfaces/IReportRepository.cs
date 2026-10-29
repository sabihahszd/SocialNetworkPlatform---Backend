using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IReportRepository :IBaseRepository<Report>
    {
        Task<Report?> GetByPublicIdAsync(string publicId);
        Task<Report?> GetOpenByPublicIdAsync(string publicId);
        Task<List<Report>> GetOpenAsync(int? reporterId = null);
        Task<List<Report>> GetOpenByPostIdAsync(int postId);
        Task<bool> HasOpenAsync(int postId, int reporterId);
        Task<int> CountOpenAsync();
    }
}

