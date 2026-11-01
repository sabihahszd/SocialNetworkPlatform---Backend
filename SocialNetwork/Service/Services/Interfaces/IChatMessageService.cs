using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IChatMessageService
    {
        Task CreateAsync(ChatMessage entity);
        Task DeleteAsync(ChatMessage entity);
        Task EditAsync(ChatMessage entity);
        Task<ChatMessage?> GetByIdAsync(int id);
        Task<IEnumerable<ChatMessage>> GetAllAsync();
        Task SaveAsync();
        Task<ChatMessage?> GetByPublicIdAsync(string publicId, int authorId);
    }
}
