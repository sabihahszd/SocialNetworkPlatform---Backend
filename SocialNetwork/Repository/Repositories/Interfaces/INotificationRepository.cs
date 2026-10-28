using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface INotificationRepository :IBaseRepository<Notification>
    {
        Task<Notification?> GetByPublicIdAsync(string publicId, int recipientId);
        Task<List<Notification>> GetForRecipientAsync(int recipientId);
        Task<List<Notification>> GetUnreadAsync(int recipientId)
    }
}
