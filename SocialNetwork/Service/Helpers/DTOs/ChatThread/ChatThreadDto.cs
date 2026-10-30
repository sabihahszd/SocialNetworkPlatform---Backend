using Service.Helpers.DTOs.ChatMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.ChatThread
{
    public class ChatThreadDto
    {
        public string Id { get; set; } = string.Empty;
        public string ParticipantId { get; set; } = string.Empty;
        public List<ChatMessageDto> Messages { get; set; } = new();
    }
}
