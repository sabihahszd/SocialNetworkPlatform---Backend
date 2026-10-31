using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IPostLikeService
    {
        Task CreateAsync(PostLike entity);
        Task DeleteAsync(PostLike entity);
        Task EditAsync(PostLike entity);
        Task<PostLike?> GetByIdAsync(int id);
        Task<IEnumerable<PostLike>> GetAllAsync();
        Task SaveAsync();
        Task<PostLike?> FindAsync(int postId, int userId);
    }
}
