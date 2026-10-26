using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Post : BaseEntity
    {
        public string PublicId { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;
        public int? CommunityId { get; set; }
        public Community? Community { get; set; }
        public string Text { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        public ICollection<PostCommunity> PostCommunities { get; set; } = new List<PostCommunity>();
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public ICollection<Repost> Reposts { get; set; } = new List<Repost>();
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();

    }
}
