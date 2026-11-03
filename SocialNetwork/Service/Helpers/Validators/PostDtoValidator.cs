using Service.Helpers.DTOs.Post;

namespace Service.Helpers.Validators
{
    public class PostDtoValidator : DtoValidator<PostDto>
    {
        public PostDtoValidator()
        {
            RuleFor(post => post.Id).NotEmpty();
            RuleFor(post => post.AuthorId).NotEmpty();
            RuleFor(post => post.Text)
                .Must(text => ValidationRules.PresentWithin(text, 1000))
                .WithMessage("Posts can be up to 1000 characters.");
            RuleFor(post => post.Privacy)
                .Must(ValidationRules.IsPrivacy)
                .WithMessage("Choose public, friends, or communities.");
            RuleFor(post => post.CommunityIds).NotNull();
            RuleFor(post => post.Likes).NotNull();
            RuleFor(post => post.Reposts).NotNull();
            RuleFor(post => post.Bookmarks).NotNull();
            RuleFor(post => post.Comments).NotNull();
            RuleForEach(post => post.Comments).SetValidator(new CommentDtoValidator());
        }
    }
}
