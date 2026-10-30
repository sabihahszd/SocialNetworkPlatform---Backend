using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.PostCommunity
{
    public class PostCommunityDto
    {
        public string PostId { get; set; } = string.Empty;
        public string CommunityId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
