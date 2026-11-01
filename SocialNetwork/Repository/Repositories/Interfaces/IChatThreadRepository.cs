using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IChatThreadRepository :IBaseRepository<ChatThread>
    {
        Task<ChatThread?> GetByPublicIdAsync(string publicId);
        Task<ChatThread?> GetBetweenAsync(int userId, int otherId);
        Task<List<ChatThread>> GetForUserAsync(int userId);
    }
}
