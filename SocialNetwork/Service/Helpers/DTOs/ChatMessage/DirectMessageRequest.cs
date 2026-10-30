using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.ChatMessage
{
    public class DirectMessageRequest
    {
        public string ParticipantId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
