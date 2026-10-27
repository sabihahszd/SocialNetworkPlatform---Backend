using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Community : BaseEntity
    {
        public string PublicId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Emoji { get; set; } = "✳";
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = "#e8efdc";
        public string BannerUrl { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public AppUser Owner { get; set; } = null!;
        public int MemberCount { get; set; }

        public ICollection<CommunityMember> Members { get; set; } = new List<CommunityMember>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
