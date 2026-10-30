using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Comment
{
    public class CommentRequestDto
    {
        public string Text { get; set; } = string.Empty;
        public string? ParentId { get; set; }
    }
}
