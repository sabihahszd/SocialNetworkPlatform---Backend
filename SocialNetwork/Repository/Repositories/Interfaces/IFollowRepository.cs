using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IFollowRepository : IBaseRepository<Follow>
    {
        Task<Follow?> FindAsync(int followerId, int followingId);
        Task<List<string>> GetFollowingPublicIdsAsync(int userId);
        Task<List<string>> GetFollowerPublicIdsAsync(int userId);
    }
}
