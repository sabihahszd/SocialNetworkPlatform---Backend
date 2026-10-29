using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string PublicId { get; set; } = string.Empty;
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public int AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;
        public int? ParentId { get; set; }
        public Comment? Parent { get; set; }
        public string Text { get; set; } = string.Empty;
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();


    }
}
