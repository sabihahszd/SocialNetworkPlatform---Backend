using Service.Helpers.DTOs.Community;

namespace Service.Helpers.Validators
{
    public class CommunityDtoValidator : DtoValidator<CommunityDto>
    {
        public CommunityDtoValidator()
        {
            RuleFor(community => community.Id).NotEmpty();
            RuleFor(community => community.Name)
                .Must(name => ValidationRules.PresentWithin(name, 80))
                .WithMessage("Community names can be up to 80 characters.");
            RuleFor(community => community.Emoji)
                .Must(emoji => ValidationRules.Within(emoji, 8))
                .WithMessage("Emoji can be up to 8 characters.");
            RuleFor(community => community.Description)
                .Must(description => ValidationRules.Within(description, 240))
                .WithMessage("Descriptions can be up to 240 characters.");
            RuleFor(community => community.Members).GreaterThanOrEqualTo(0);
            RuleFor(community => community.MemberIds).NotNull();
            RuleFor(community => community.OwnerId).NotEmpty();
            RuleFor(community => community.Color)
                .Must(ValidationRules.IsHexColor)
                .WithMessage("Choose a hex color.");
        }
    }
}
