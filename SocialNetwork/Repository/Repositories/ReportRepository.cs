using Domain.Entities;
using Domain.Enums;
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
            return DbSet.CountAsync(report => !report.IsDeleted && report.Status == ReportStatus.Open);
        }

        public Task<Report?> GetByPublicIdAsync(string publicId)
        {
            return DbSet.AsNoTracking()
                            .Include(report => report.Post).ThenInclude(post => post!.Author)
                            .Include(report => report.ReportedUser)
                            .Include(report => report.Community)
                            .Include(report => report.Reporter)
                            .FirstOrDefaultAsync(report => report.PublicId == publicId);
        }

        public Task<List<Report>> GetOpenAsync(int? reporterId = null)
        {
            var reports = DbSet.AsNoTracking()
                            .Include(report => report.Post).ThenInclude(post => post!.Author)
                            .Include(report => report.ReportedUser)
                            .Include(report => report.Community)
                            .Include(report => report.Reporter)
                            .Where(report => report.Status == ReportStatus.Open);
            if (reporterId is not null) reports = reports.Where(report => report.ReporterId == reporterId);
            return reports.OrderByDescending(report => report.CreatedAt).ToListAsync();
        }        

        public Task<List<Report>> GetOpenByPostIdAsync(int postId)
        {
            return DbSet.Where(report => report.PostId == postId && report.Status == ReportStatus.Open).ToListAsync();
        }

        public Task<Report?> GetOpenByPublicIdAsync(string publicId)
        {
            return DbSet.Include(report => report.Post)
                           .Include(report => report.ReportedUser)
                           .Include(report => report.Community)
                           .FirstOrDefaultAsync(report => report.PublicId == publicId && report.Status == ReportStatus.Open);
        }

        public Task<bool> HasOpenAsync(int postId, int reporterId)
        {
            return DbSet.AnyAsync(report => report.PostId == postId && report.ReporterId == reporterId && report.Status == ReportStatus.Open);
        }

        public Task<List<Report>> GetOpenByUserIdAsync(int userId)
        {
            return DbSet.Where(report => report.ReportedUserId == userId && report.Status == ReportStatus.Open).ToListAsync();
        }

        public Task<List<Report>> GetOpenByCommunityIdAsync(int communityId)
        {
            return DbSet.Where(report => report.CommunityId == communityId && report.Status == ReportStatus.Open).ToListAsync();
        }
    }
}
