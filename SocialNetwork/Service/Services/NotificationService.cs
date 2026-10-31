using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public Task CreateAsync(Notification entity)
        {
            return _notificationRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(Notification entity)
        {
            return _notificationRepository.DeleteAsync(entity);
        }

        public Task EditAsync(Notification entity)
        {
            return _notificationRepository.EditAsync(entity);
        }

        public Task<Notification?> GetByIdAsync(int id)
        {
            return _notificationRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Notification>> GetAllAsync()
        {
            return _notificationRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _notificationRepository.SaveAsync();
        }

        public Task<Notification?> GetByPublicIdAsync(string publicId, int recipientId)
        {
            return _notificationRepository.GetByPublicIdAsync(publicId, recipientId);
        }

        public Task<List<Notification>> GetForRecipientAsync(int recipientId)
        {
            return _notificationRepository.GetForRecipientAsync(recipientId);
        }

        public Task<List<Notification>> GetUnreadAsync(int recipientId)
        {
            return _notificationRepository.GetUnreadAsync(recipientId);
        }
    }
}
