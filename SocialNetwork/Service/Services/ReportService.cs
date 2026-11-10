using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public Task CreateAsync(Report entity)
        {
            return _reportRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Report entity)
        {
            return _reportRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Report entity)
        {
            return _reportRepository.EditAsync(entity);
        }

        public Task<Report?> GetByIdAsync(int id)
        {
            return _reportRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Report>> GetAllAsync()
        {
            return _reportRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _reportRepository.SaveAsync();
        }

        public Task<Report?> GetByPublicIdAsync(string publicId)
        {
            return _reportRepository.GetByPublicIdAsync(publicId);
        }

        public Task<Report?> GetOpenByPublicIdAsync(string publicId)
        {
            return _reportRepository.GetOpenByPublicIdAsync(publicId);
        }

        public Task<List<Report>> GetOpenAsync(int? reporterId = null)
        {
            return _reportRepository.GetOpenAsync(reporterId);
        }

        public Task<List<Report>> GetOpenByPostIdAsync(int postId)
        {
            return _reportRepository.GetOpenByPostIdAsync(postId);
        }

        public Task<bool> HasOpenAsync(int postId, int reporterId)
        {
            return _reportRepository.HasOpenAsync(postId, reporterId);
        }

        public Task<List<Report>> GetOpenByUserIdAsync(int userId)
        {
            return _reportRepository.GetOpenByUserIdAsync(userId);
        }

        public Task<List<Report>> GetOpenByCommunityIdAsync(int communityId)
        {
            return _reportRepository.GetOpenByCommunityIdAsync(communityId);
        }

        public Task<int> CountOpenAsync()
        {
            return _reportRepository.CountOpenAsync();
        }
    }
}
