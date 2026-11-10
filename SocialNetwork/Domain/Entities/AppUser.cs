using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string PublicId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PasswordResetCodeHash { get; set; }
        public DateTime? PasswordResetExpiresAt { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string BannerUrl { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Member;
        public string Tone { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsBanned { get; set; }
        public DateTime? SuspendedUntil { get; set; }

        public UserSettings? Settings { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<CommunityMember> CommunityMemberships { get; set; } = new List<CommunityMember>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}

