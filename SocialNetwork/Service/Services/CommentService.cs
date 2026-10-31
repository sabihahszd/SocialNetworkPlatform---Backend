using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public Task CreateAsync(Comment entity)
        {
            return _commentRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Comment entity)
        {
            return _commentRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Comment entity)
        {
            return _commentRepository.EditAsync(entity);
        }

        public Task<Comment?> GetByIdAsync(int id)
        {
            return _commentRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Comment>> GetAllAsync()
        {
            return _commentRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _commentRepository.SaveAsync();
        }

        public Task<Comment?> GetByPublicIdAsync(string publicId)
        {
            return _commentRepository.GetByPublicIdAsync(publicId);
        }

        public Task<Comment?> GetTrackedByPublicIdAsync(string publicId)
        {
            return _commentRepository.GetTrackedByPublicIdAsync(publicId);
        }
    }
}
