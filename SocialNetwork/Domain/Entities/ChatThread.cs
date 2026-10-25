using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatThread : BaseEntity
    {
        public string PublicId { get; set; } = string.Empty;
        public int UserAId { get; set; }
        public AppUser UserA { get; set; } = null!;
        public int UserBId { get; set; }
        public AppUser UserB { get; set; } = null!;
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
