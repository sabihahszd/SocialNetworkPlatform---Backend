using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PostCommunity : BaseEntity 
    {
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public int CommunityId { get; set; }
        public Community Community { get; set; } = null!;
    }
}
