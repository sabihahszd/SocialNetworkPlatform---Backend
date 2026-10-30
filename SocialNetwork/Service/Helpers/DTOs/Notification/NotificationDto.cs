using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Notification
{
    public class NotificationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ActorId { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Read { get; set; }
        public string Status { get; set; } = "done";
        public string? Detail { get; set; }
    }
}
