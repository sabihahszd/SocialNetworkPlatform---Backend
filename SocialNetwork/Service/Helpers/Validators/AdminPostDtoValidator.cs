using Service.Helpers.DTOs.Post;

namespace Service.Helpers.Validators
{
    public class AdminPostDtoValidator : DtoValidator<AdminPostDto>
    {
        public AdminPostDtoValidator()
        {
            RuleFor(post => post.Id).NotEmpty();
            RuleFor(post => post.AuthorId).NotEmpty();
            RuleFor(post => post.AuthorName).NotEmpty();
            RuleFor(post => post.AuthorUsername).NotEmpty();
            RuleFor(post => post.Text)
                .Must(text => ValidationRules.PresentWithin(text, 1000))
                .WithMessage("Posts can be up to 1000 characters.");
            RuleFor(post => post.Privacy)
                .Must(ValidationRules.IsPrivacy)
                .WithMessage("Choose public, friends, or communities.");
        }
    }
}
