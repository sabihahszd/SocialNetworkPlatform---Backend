using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string PublicId { get; set; } = string.Empty;
        public int RecipientId { get; set; }
        public AppUser Recipient { get; set; } = null!;
        public int ActorId { get; set; }
        public AppUser Actor { get; set; } = null!;
        public NotificationType Type { get; set; }
        public string? PostPublicId { get; set; }
        public string Detail { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Done;
    }
}
