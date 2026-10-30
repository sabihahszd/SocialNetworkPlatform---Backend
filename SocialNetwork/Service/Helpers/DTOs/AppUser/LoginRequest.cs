using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.AppUser
{
    public class LoginRequest
    {
        public string Identity { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

