using Service.Helpers.DTOs.AppUser;

namespace Service.Helpers.Validators
{
    public class UserDtoValidator : DtoValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(user => user.Id).NotEmpty();
            RuleFor(user => user.Name)
                .Must(name => ValidationRules.PresentWithin(name, 60))
                .WithMessage("Names can be up to 60 characters.");
            RuleFor(user => user.Username)
                .Must(username => ValidationRules.PresentWithin(username, 24) && !ValidationRules.Trimmed(username).Contains(' '))
                .WithMessage("Choose a username up to 24 characters without spaces.");
            RuleFor(user => user.Email)
                .Must(email => email is null || ValidationRules.IsEmail(email))
                .WithMessage("Enter a valid email address.");
            RuleFor(user => user.Bio).Must(bio => ValidationRules.Within(bio, 180));
            RuleFor(user => user.Location).Must(location => ValidationRules.Within(location, 48));
            RuleFor(user => user.Role).Must(ValidationRules.IsRole).WithMessage("Choose member, admin, or superadmin.");
        }
    }
}
