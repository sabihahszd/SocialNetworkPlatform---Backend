using Service.Helpers.DTOs.Friendship;

namespace Service.Helpers.Validators
{
    public class FriendshipDtoValidator : DtoValidator<FriendshipDto>
    {
        public FriendshipDtoValidator()
        {
            RuleFor(friendship => friendship.UserId).NotEmpty();
            RuleFor(friendship => friendship.FriendId).NotEmpty();
        }
    }
}
