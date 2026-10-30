using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.DTOs.SiteSettings
{
    public class UpdateSiteSettingsRequest
    {
        public string? SiteName { get; set; }
        public string? Tagline { get; set; }
        public string? LogoMark { get; set; }
        public string? FooterNote { get; set; }
        public string? LogoUrl { get; set; }
        public string? AboutBody { get; set; }
        public string? GuidelinesBody { get; set; }
        public string? PrivacyBody { get; set; }
    }
}
