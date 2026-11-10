using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.AppUser
{
    public class UpdateProfileRequest
    {
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? Photo { get; set; }
        public string? Banner { get; set; }
        public string? Password { get; set; }
        public string? CurrentPassword { get; set; }
    }
}
