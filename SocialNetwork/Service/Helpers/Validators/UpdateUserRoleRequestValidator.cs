using Service.Helpers.DTOs.AppUser;

namespace Service.Helpers.Validators
{
    public class UpdateUserRoleRequestValidator : DtoValidator<UpdateUserRoleRequest>
    {
        public UpdateUserRoleRequestValidator()
        {
            RuleFor(request => request.Role)
                .Must(ValidationRules.IsAssignableRole)
                .WithMessage("Choose member or admin.");
        }
    }
}
