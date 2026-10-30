using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Post
{
    public class CreatePostRequest
    {
        public string Text { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Video { get; set; }
        public string Privacy { get; set; } = "public";
        public string? CommunityId { get; set; }
        public List<string>? CommunityIds { get; set; }
    }
}
