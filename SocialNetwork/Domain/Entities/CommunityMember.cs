using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CommunityMember : BaseEntity
    {
        public int CommunityId { get; set; }
        public Community Community { get; set; } = null!;
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
