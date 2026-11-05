using Service.Helpers.DTOs.AppUser;

namespace Service.Helpers.Validators
{
    public class RegisterRequestValidator : DtoValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(request => request.FullName)
                .Must(name => !ValidationRules.IsBlank(name))
                .WithMessage("Enter your name.")
                .Must(name => ValidationRules.PresentWithin(name, 60))
                .WithMessage("Names can be up to 60 characters.");

            RuleFor(request => request.Username)
                .Must(username => !ValidationRules.IsBlank(username))
                .WithMessage("Choose a username.")
                .Must(username => !ValidationRules.Trimmed(username).Contains(' '))
                .WithMessage("Usernames cannot contain spaces.")
                .Must(username => ValidationRules.PresentWithin(username, 24))
                .WithMessage("Usernames can be up to 24 characters.");

            RuleFor(request => request.Email)
                .Must(email => !ValidationRules.IsBlank(email))
                .WithMessage("Enter your email address.")
                .Must(ValidationRules.IsEmail)
                .WithMessage("Enter a valid email address.");

            RuleFor(request => request.Password)
                .Must(password => !ValidationRules.IsBlank(password))
                .WithMessage("Enter a password.")
                .Must(password => ValidationRules.Trimmed(password).Length >= 6)
                .WithMessage("Password must be at least 6 characters.");

            RuleFor(request => request.ConfirmPassword)
                .Must(confirm => !ValidationRules.IsBlank(confirm))
                .WithMessage("Confirm your password.")
                .Must((request, confirm) => confirm == request.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(request => request.Bio)
                .Must(bio => ValidationRules.Within(bio, 180))
                .WithMessage("Bio can be up to 180 characters.");

            RuleFor(request => request.Location)
                .Must(location => ValidationRules.Within(location, 48))
                .WithMessage("Location can be up to 48 characters.");
        }
    }
}
