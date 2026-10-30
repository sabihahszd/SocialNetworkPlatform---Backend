using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SiteSettings : BaseEntity
    {
        public string SiteName { get; set; } = "gather";
        public string Tagline { get; set; } = "Your people, your place.";
        public string LogoMark { get; set; } = "g";
        public string LogoUrl { get; set; } = string.Empty;
        public string FooterNote { get; set; } = "Small moments, shared well.";
        public string AboutBody { get; set; } = string.Empty;
        public string GuidelinesBody { get; set; } = string.Empty;
        public string PrivacyBody { get; set; } = string.Empty;
    }
}
