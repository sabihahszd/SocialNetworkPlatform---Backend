using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IChatThreadService
    {
        Task CreateAsync(ChatThread entity);
        Task DeleteAsync(ChatThread entity);
        Task EditAsync(ChatThread entity);
        Task<ChatThread?> GetByIdAsync(int id);
        Task<IEnumerable<ChatThread>> GetAllAsync();
        Task SaveAsync();
        Task<ChatThread?> GetByPublicIdAsync(string publicId);
        Task<ChatThread?> GetBetweenAsync(int userId, int otherId);
        Task<List<ChatThread>> GetForUserAsync(int userId);
    }
}
