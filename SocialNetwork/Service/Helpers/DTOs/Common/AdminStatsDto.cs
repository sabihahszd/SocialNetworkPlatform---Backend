using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.Common
{
    public class AdminStatsDto
    {
        public int Members { get; set; }
        public int Posts { get; set; }
        public int Reports { get; set; }
        public int Circles { get; set; }
    }
}

