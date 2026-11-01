using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IFriendRequestRepository : IBaseRepository<FriendRequest>
    {
        Task<FriendRequest?> FindAsync(int fromUserId, int toUserId);
        Task<FriendRequest?> GetPendingAsync(int fromUserId, int toUserId);
        Task<List<string>> GetOutgoingPublicIdsAsync(int userId);
    }
}
