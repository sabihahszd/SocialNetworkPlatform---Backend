using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Community
{
    public class CommunityDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Emoji { get; set; } = "✳";
        public string Description { get; set; } = string.Empty;
        public int Members { get; set; }
        public List<string> MemberIds { get; set; } = new();
        public string OwnerId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public bool Joined { get; set; }
        public string Color { get; set; } = "#e8efdc";
        public string Banner { get; set; } = string.Empty;
    }
}
