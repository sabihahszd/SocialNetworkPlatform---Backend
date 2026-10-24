using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Friendship : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int FriendId { get; set; }
        public AppUser Friend { get; set; } = null!;
    }
}
