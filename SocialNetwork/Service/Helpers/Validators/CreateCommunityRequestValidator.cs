using Service.Helpers.DTOs.Community;

namespace Service.Helpers.Validators
{
    public class CreateCommunityRequestValidator : DtoValidator<CreateCommunityRequest>
    {
        public const string Member = "Member";
        public const string Admin = "Admin";

        public CreateCommunityRequestValidator()
        {
            RuleSet(Member, () =>
            {
                RuleFor(request => request.Name)
                    .Must(name => !ValidationRules.IsBlank(name))
                    .WithMessage("Give the community a name.")
                    .Must(name => ValidationRules.PresentWithin(name, 32))
                    .WithMessage("Community names can be up to 32 characters.");

                RuleFor(request => request.Description)
                    .Must(description => ValidationRules.Within(description, 120))
                    .WithMessage("Descriptions can be up to 120 characters.");
            });

            RuleSet(Admin, () =>
            {
                RuleFor(request => request.Name)
                    .Must(name => !ValidationRules.IsBlank(name))
                    .WithMessage("Give the community a name.")
                    .Must(name => ValidationRules.PresentWithin(name, 80))
                    .WithMessage("Community names can be up to 80 characters.");

                RuleFor(request => request.Description)
                    .Must(description => ValidationRules.Within(description, 240))
                    .WithMessage("Descriptions can be up to 240 characters.");

                RuleFor(request => request.Emoji)
                    .Must(emoji => ValidationRules.Within(emoji, 8))
                    .WithMessage("Emoji can be up to 8 characters.");

                RuleFor(request => request.Color)
                    .Must(ValidationRules.IsOptionalHexColor)
                    .WithMessage("Choose a hex color.");
            });
        }
    }
}
