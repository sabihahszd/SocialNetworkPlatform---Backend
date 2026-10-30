using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Friendship
{
    public class FriendshipDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FriendId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
