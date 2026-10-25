using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FriendRequest : BaseEntity
    {
        public int FromUserId { get; set; }
        public AppUser FromUser { get; set; } = null!;
        public int ToUserId { get; set; }
        public AppUser ToUser { get; set; } = null!;
    }
}
