using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserSettings : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public string DefaultPrivacy { get; set; } = "public";
        public bool AllowRequests { get; set; } = true;
        public bool ShowSuggestions { get; set; } = true;
        public bool AllowDms { get; set; } = true;
        public bool NotifyLikes { get; set; } = true;
        public bool NotifyReplies { get; set; } = true;
        public bool NotifyReposts { get; set; } = true;
        public bool NotifyRequests { get; set; } = true;
        public bool NotifyCommunities { get; set; } = true;
        public string Theme { get; set; } = "system";
        public bool ReduceMotion { get; set; }
        public bool Autoplay { get; set; } = true;
        public string BlockedUserPublicIds { get; set; } = "[]";
    }
}

