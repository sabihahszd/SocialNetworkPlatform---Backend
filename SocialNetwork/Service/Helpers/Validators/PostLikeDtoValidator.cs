using Service.Helpers.DTOs.PostLike;

namespace Service.Helpers.Validators
{
    public class PostLikeDtoValidator : DtoValidator<PostLikeDto>
    {
        public PostLikeDtoValidator()
        {
            RuleFor(like => like.PostId).NotEmpty();
            RuleFor(like => like.UserId).NotEmpty();
        }
    }
}
