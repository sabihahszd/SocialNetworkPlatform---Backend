using Service.Helpers.DTOs.AppUser;

namespace Service.Helpers.Validators
{
    public class ForgotPasswordRequestValidator : DtoValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator()
        {
            RuleFor(request => request.Email)
                .Must(email => !ValidationRules.IsBlank(email))
                .WithMessage("Enter your email address.")
                .Must(ValidationRules.IsEmail)
                .WithMessage("Enter a valid email address.");
        }
    }
}
