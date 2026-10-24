using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Follow : BaseEntity
    {
        public int FollowerId { get; set; }
        public AppUser Follower { get; set; } = null!;
        public int FollowingId { get; set; }
        public AppUser Following { get; set; } = null!;
    }
}
