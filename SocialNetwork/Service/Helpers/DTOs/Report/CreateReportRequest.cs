using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Report
{
    public class CreateReportRequest
    {
        public string PostId { get; set; } = string.Empty;
        public string? CommentId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
