using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.AppUser
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
        public string Tone { get; set; } = string.Empty;
        public string Role { get; set; } = "member";
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
