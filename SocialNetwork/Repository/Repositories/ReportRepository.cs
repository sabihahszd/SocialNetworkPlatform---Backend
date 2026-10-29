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
    public class ReportRepository : BaseRepository<Report>, IReportRepository
    {
        public ReportRepository(AppDbContext context) : base(context)
        {
        }

        public Task<int> CountOpenAsync()
        {
            return DbSet.CountAsync(report => report.Status == ReportStatus.Open);
        }

        public Task<Report?> GetByPublicIdAsync(string publicId)
        {
            return DbSet.AsNoTracking()
                            .Include(report => report.Post).ThenInclude(post => post.Author)
                            .Include(report => report.Reporter)
                            .FirstOrDefaultAsync(report => report.PublicId == publicId);
        }

        public Task<List<Report>> GetOpenAsync(int? reporterId = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<Report>> GetOpenByPostIdAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<Report?> GetOpenByPublicIdAsync(string publicId)
        {
            return DbSet.Where(report => report.PostId == postId && report.Status == ReportStatus.Open).ToListAsync();
        }

        public Task<bool> HasOpenAsync(int postId, int reporterId)
        {
            return DbSet.AnyAsync(report => report.PostId == postId && report.ReporterId == reporterId && report.Status == ReportStatus.Open);
        }
    }
}
