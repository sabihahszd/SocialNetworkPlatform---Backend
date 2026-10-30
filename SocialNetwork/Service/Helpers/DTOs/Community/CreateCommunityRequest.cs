using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Community
{
    public class CreateCommunityRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Emoji { get; set; }
        public string? Color { get; set; }
        public string? Banner { get; set; }
    }
}
