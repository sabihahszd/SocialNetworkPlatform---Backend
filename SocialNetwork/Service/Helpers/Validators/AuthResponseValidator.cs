using Service.Helpers.DTOs.AppUser;

namespace Service.Helpers.Validators
{
    public class AuthResponseValidator : DtoValidator<AuthResponse>
    {
        public AuthResponseValidator()
        {
            RuleFor(response => response.Token).NotEmpty();
            RuleFor(response => response.User).NotNull().SetValidator(new UserDtoValidator());
        }
    }
}
