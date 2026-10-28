using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IPostCommunityRepository : IBaseRepository<PostCommunity>
    {
        Task<bool> ExistsAsync(int postId, int communityId);

    }
}
