using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(Notification entity);
        Task DeleteAsync(Notification entity);
        Task EditAsync(Notification entity);
        Task<Notification?> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetAllAsync();
        Task SaveAsync();
        Task<Notification?> GetByPublicIdAsync(string publicId, int recipientId);
        Task<List<Notification>> GetForRecipientAsync(int recipientId);
        Task<List<Notification>> GetUnreadAsync(int recipientId);
    }
}
