using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class FriendRequestRepository : BaseRepository<FriendRequest>, IFriendRequestRepository
    {
        public FriendRequestRepository(AppDbContext context) : base(context)
        {
        }

        public Task<FriendRequest?> FindAsync(int fromUserId, int toUserId)
        {
            return DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(request => request.FromUserId == fromUserId && request.ToUserId == toUserId);
        }

        public Task<List<string>> GetOutgoingPublicIdsAsync(int userId)
        {
            return DbSet.Where(request => request.FromUserId == userId && request.Status == RequestStatus.Pending)
                            .Select(request => request.ToUser.PublicId)
                            .ToListAsync();
        }
    

        public Task<FriendRequest?> GetPendingAsync(int fromUserId, int toUserId)
        {
            return DbSet.FirstOrDefaultAsync(request => request.FromUserId == fromUserId && request.ToUserId == toUserId && request.Status == RequestStatus.Pending);
        }
    }
}
