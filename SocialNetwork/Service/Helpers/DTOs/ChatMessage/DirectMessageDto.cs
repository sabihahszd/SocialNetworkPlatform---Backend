using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.ChatMessage
{
    public class DirectMessageDto
    {
        public string ThreadId { get; set; } = string.Empty;
        public string ParticipantId { get; set; } = string.Empty;
        public ChatMessageDto Message { get; set; } = new();
    }
}
