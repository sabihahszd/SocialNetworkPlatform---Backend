using Service.Helpers.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Post
{
    public class PostDto
    {
        public string Id { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string? CommunityId { get; set; }
        public List<string> CommunityIds { get; set; } = new();
        public string Text { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Video { get; set; } = string.Empty;
        public string Privacy { get; set; } = "public";
        public DateTime CreatedAt { get; set; }
        public List<string> Likes { get; set; } = new();
        public List<string> Reposts { get; set; } = new();
        public List<string> Bookmarks { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
    }
}

