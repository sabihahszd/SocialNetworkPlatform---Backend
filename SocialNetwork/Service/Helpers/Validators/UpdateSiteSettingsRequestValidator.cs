using Service.Helpers.DTOs.SiteSettings;

namespace Service.Helpers.Validators
{
    public class UpdateSiteSettingsRequestValidator : DtoValidator<UpdateSiteSettingsRequest>
    {
        public UpdateSiteSettingsRequestValidator()
        {
            RuleFor(request => request.SiteName)
                .Must(name => ValidationRules.PresentWithin(name, 40))
                .When(request => request.SiteName is not null)
                .WithMessage("Site name can be up to 40 characters.");

            RuleFor(request => request.Tagline)
                .Must(tagline => ValidationRules.Within(tagline, 120))
                .When(request => request.Tagline is not null)
                .WithMessage("Tagline can be up to 120 characters.");

            RuleFor(request => request.LogoMark)
                .Must(mark => ValidationRules.Within(mark, 2))
                .When(request => request.LogoMark is not null)
                .WithMessage("Logo letter can be up to 2 characters.");

            RuleFor(request => request.FooterNote)
                .Must(footer => ValidationRules.Within(footer, 160))
                .When(request => request.FooterNote is not null)
                .WithMessage("Footer note can be up to 160 characters.");

            RuleFor(request => request.AboutBody)
                .Must(body => ValidationRules.Within(body, 2000))
                .When(request => request.AboutBody is not null)
                .WithMessage("About can be up to 2000 characters.");

            RuleFor(request => request.GuidelinesBody)
                .Must(body => ValidationRules.Within(body, 2000))
                .When(request => request.GuidelinesBody is not null)
                .WithMessage("Guidelines can be up to 2000 characters.");

            RuleFor(request => request.PrivacyBody)
                .Must(body => ValidationRules.Within(body, 2000))
                .When(request => request.PrivacyBody is not null)
                .WithMessage("Privacy can be up to 2000 characters.");
        }
    }
}
