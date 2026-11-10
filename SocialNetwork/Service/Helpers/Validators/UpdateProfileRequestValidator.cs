using Service.Helpers.DTOs.AppUser;

namespace Service.Helpers.Validators
{
    public class UpdateProfileRequestValidator : DtoValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator()
        {
            RuleFor(request => request.Name)
                .Must(name => ValidationRules.PresentWithin(name, 60))
                .When(request => request.Name is not null)
                .WithMessage("Names can be up to 60 characters.");

            RuleFor(request => request.Username)
                .Must(username => ValidationRules.PresentWithin(username, 24) && !ValidationRules.Trimmed(username).Contains(' '))
                .When(request => request.Username is not null)
                .WithMessage("Usernames can be up to 24 characters and cannot contain spaces.");

            RuleFor(request => request.Email)
                .Must(ValidationRules.IsEmail)
                .When(request => request.Email is not null)
                .WithMessage("Enter a valid email address.");

            RuleFor(request => request.Bio)
                .Must(bio => ValidationRules.Within(bio, 180))
                .When(request => request.Bio is not null)
                .WithMessage("Bio can be up to 180 characters.");

            RuleFor(request => request.Location)
                .Must(location => ValidationRules.Within(location, 48))
                .When(request => request.Location is not null)
                .WithMessage("Location can be up to 48 characters.");

            RuleFor(request => request.Password)
                .Must(password => ValidationRules.IsBlank(password) || ValidationRules.IsStrongPassword(password))
                .WithMessage(ValidationRules.StrongPasswordMessage);

            RuleFor(request => request.CurrentPassword)
                .Must(password => !ValidationRules.IsBlank(password))
                .When(request => request.Username is not null || request.Email is not null || !ValidationRules.IsBlank(request.Password))
                .WithMessage("Enter your current password.");
        }
    }
}
