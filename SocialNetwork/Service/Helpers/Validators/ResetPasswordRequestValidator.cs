using Service.Helpers.DTOs.AppUser;

namespace Service.Helpers.Validators
{
    public class ResetPasswordRequestValidator : DtoValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(request => request.Email)
                .Must(email => !ValidationRules.IsBlank(email))
                .WithMessage("Enter your email address.")
                .Must(ValidationRules.IsEmail)
                .WithMessage("Enter a valid email address.");

            RuleFor(request => request.Code)
                .Must(code => ValidationRules.Trimmed(code).Length == 6 && ValidationRules.Trimmed(code).All(char.IsDigit))
                .WithMessage("Enter the 6-digit verification code.");

            RuleFor(request => request.Password)
                .Must(password => !ValidationRules.IsBlank(password))
                .WithMessage("Enter a new password.")
                .Must(password => ValidationRules.Trimmed(password).Length >= 6)
                .WithMessage("Password must be at least 6 characters.");

            RuleFor(request => request.ConfirmPassword)
                .Must(confirm => !ValidationRules.IsBlank(confirm))
                .WithMessage("Confirm your new password.")
                .Must((request, confirm) => confirm == request.Password)
                .WithMessage("Passwords do not match.");
        }
    }
}
