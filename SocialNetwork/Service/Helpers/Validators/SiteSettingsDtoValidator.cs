using Service.Helpers.DTOs.SiteSettings;

namespace Service.Helpers.Validators
{
    public class SiteSettingsDtoValidator : DtoValidator<SiteSettingsDto>
    {
        public SiteSettingsDtoValidator()
        {
            RuleFor(settings => settings.SiteName)
                .Must(name => ValidationRules.PresentWithin(name, 40))
                .WithMessage("Site name can be up to 40 characters.");

            RuleFor(settings => settings.Tagline)
                .Must(tagline => ValidationRules.Within(tagline, 120))
                .WithMessage("Tagline can be up to 120 characters.");

            RuleFor(settings => settings.LogoMark)
                .Must(mark => ValidationRules.Within(mark, 2))
                .WithMessage("Logo letter can be up to 2 characters.");

            RuleFor(settings => settings.FooterNote)
                .Must(footer => ValidationRules.Within(footer, 160))
                .WithMessage("Footer note can be up to 160 characters.");

            RuleFor(settings => settings.AboutBody)
                .Must(body => ValidationRules.Within(body, 2000))
                .WithMessage("About can be up to 2000 characters.");

            RuleFor(settings => settings.GuidelinesBody)
                .Must(body => ValidationRules.Within(body, 2000))
                .WithMessage("Guidelines can be up to 2000 characters.");

            RuleFor(settings => settings.PrivacyBody)
                .Must(body => ValidationRules.Within(body, 2000))
                .WithMessage("Privacy can be up to 2000 characters.");
        }
    }
}
