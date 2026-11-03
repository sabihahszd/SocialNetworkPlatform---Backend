using Service.Helpers.DTOs.Post;

namespace Service.Helpers.Validators
{
    public class CreatePostRequestValidator : DtoValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(request => request.Text)
                .Must(text => !ValidationRules.IsBlank(text))
                .WithMessage("Write something before sharing.")
                .Must(text => ValidationRules.PresentWithin(text, 1000))
                .WithMessage("Posts can be up to 1000 characters.");

            RuleFor(request => request.Privacy)
                .Must(ValidationRules.IsOptionalPrivacy)
                .WithMessage("Choose public, friends, or communities.");
        }
    }
}
