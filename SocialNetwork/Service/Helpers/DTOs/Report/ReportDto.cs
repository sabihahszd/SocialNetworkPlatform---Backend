using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Report
{
    public class ReportDto
    {
        public string Id { get; set; } = string.Empty;
        public string Kind { get; set; } = "post";
        public string PostId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? ReportedName { get; set; }
        public string? Username { get; set; }
        public string CommunityId { get; set; } = string.Empty;
        public string? CommunityName { get; set; }
        public string ReporterId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "open";
        public string? PostText { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorUsername { get; set; }
        public string? ReporterName { get; set; }
    }
}
