using Service.Helpers.DTOs.FriendRequest;

namespace Service.Helpers.Validators
{
    public class FriendRequestDtoValidator : DtoValidator<FriendRequestDto>
    {
        public FriendRequestDtoValidator()
        {
            RuleFor(request => request.FromUserId).NotEmpty();
            RuleFor(request => request.ToUserId).NotEmpty();
            RuleFor(request => request.Status)
                .Must(ValidationRules.IsRequestStatus)
                .WithMessage("Choose pending, accepted, declined, or done.");
        }
    }
}
