using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        public string PublicId { get; set; } = string.Empty;
        public int ThreadId { get; set; }
        public ChatThread Thread { get; set; } = null!;
        public int AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}
