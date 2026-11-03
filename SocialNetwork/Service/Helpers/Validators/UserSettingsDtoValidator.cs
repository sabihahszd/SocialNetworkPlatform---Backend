using Service.Helpers.DTOs.UserSettings;

namespace Service.Helpers.Validators
{
    public class UserSettingsDtoValidator : DtoValidator<UserSettingsDto>
    {
        public UserSettingsDtoValidator()
        {
            RuleFor(settings => settings.DefaultPrivacy)
                .Must(ValidationRules.IsPrivacy)
                .WithMessage("Choose public, friends, or communities.");

            RuleFor(settings => settings.Theme)
                .Must(ValidationRules.IsTheme)
                .WithMessage("Choose system, light, or dark.");
        }
    }
}
