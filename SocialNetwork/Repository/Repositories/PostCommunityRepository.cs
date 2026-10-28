using Domain.Entities;
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
    public class PostCommunityRepository : BaseRepository<PostCommunity>, IPostCommunityRepository
    {
        public PostCommunityRepository(AppDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(int postId, int communityId)
        {
            return DbSet.AnyAsync(link => link.PostId == postId && link.CommunityId == communityId);
        }
    }
}
