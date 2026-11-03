using Service.Helpers.DTOs.Repost;

namespace Service.Helpers.Validators
{
    public class RepostDtoValidator : DtoValidator<RepostDto>
    {
        public RepostDtoValidator()
        {
            RuleFor(repost => repost.PostId).NotEmpty();
            RuleFor(repost => repost.UserId).NotEmpty();
        }
    }
}
