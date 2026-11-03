using Service.Helpers.DTOs.Follow;

namespace Service.Helpers.Validators
{
    public class FollowDtoValidator : DtoValidator<FollowDto>
    {
        public FollowDtoValidator()
        {
            RuleFor(follow => follow.FollowerId).NotEmpty();
            RuleFor(follow => follow.FollowingId).NotEmpty();
        }
    }
}
