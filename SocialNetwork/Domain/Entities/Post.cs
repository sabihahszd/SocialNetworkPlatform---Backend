using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    internal class Post
    {
        public string PublicId { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;
        public int? CommunityId { get; set; }
        public Community? Community { get; set; }
        public string Text { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
    }
}
