using Service.Helpers.DTOs.PostCommunity;

namespace Service.Helpers.Validators
{
    public class PostCommunityDtoValidator : DtoValidator<PostCommunityDto>
    {
        public PostCommunityDtoValidator()
        {
            RuleFor(link => link.PostId).NotEmpty();
            RuleFor(link => link.CommunityId).NotEmpty();
        }
    }
}
