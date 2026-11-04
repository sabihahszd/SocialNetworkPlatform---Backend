using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IFriendshipRepository : IBaseRepository<Friendship>
    {
        Task<bool> ExistsAsync(int userId, int friendId);
        Task<Friendship?> FindAsync(int userId, int friendId);
        Task<List<string>> GetFriendPublicIdsAsync(int userId);
    }
}
