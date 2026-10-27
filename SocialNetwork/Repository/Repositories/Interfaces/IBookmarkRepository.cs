using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IBookmarkRepository : IBaseRepository<Bookmark>
    {
        Task<Bookmark?> FindAsync(int postId, int userId);

    }
}
