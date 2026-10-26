using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IPostRepository : IBaseRepository<Post> 
    {
        Task<Post?> GetByPublicIdAsync(string publicId);
        Task<Post?> GetTrackedByPublicIdAsync(string publicId);
        Task<Post?> GetDetailedByIdAsync(int id);
        Task<List<Post>> GetFeedAsync();
        Task<List<Post>> SearchAsync(string? query);
        Task<int> CountAsync();
    }
}
