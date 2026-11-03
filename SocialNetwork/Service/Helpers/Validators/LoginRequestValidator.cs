using Service.Helpers.DTOs.AppUser;

namespace Service.Helpers.Validators
{
    public class LoginRequestValidator : DtoValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(request => request.Identity)
                .Must(identity => !ValidationRules.IsBlank(identity))
                .WithMessage("Enter your email or username.");

            RuleFor(request => request.Password)
                .Must(password => !ValidationRules.IsBlank(password))
                .WithMessage("Enter your password.");
        }
    }
}
