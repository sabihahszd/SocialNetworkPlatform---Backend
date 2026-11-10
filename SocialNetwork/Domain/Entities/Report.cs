using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Report : BaseEntity
    {
        public string PublicId { get; set; } = string.Empty;
        public int? PostId { get; set; }
        public Post? Post { get; set; }
        public int? ReportedUserId { get; set; }
        public AppUser? ReportedUser { get; set; }
        public int? CommunityId { get; set; }
        public Community? Community { get; set; }
        public int ReporterId { get; set; }
        public AppUser Reporter { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
        public ReportStatus Status { get; set; } = ReportStatus.Open;
    }
}
