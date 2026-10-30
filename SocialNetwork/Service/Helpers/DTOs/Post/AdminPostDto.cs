using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Post
{
    public class AdminPostDto
    {
        public string Id { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorUsername { get; set; } = string.Empty;
        public string? CommunityName { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Privacy { get; set; } = "public";
        public DateTime CreatedAt { get; set; }
    }
}
