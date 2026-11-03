using Service.Helpers.DTOs.Comment;

namespace Service.Helpers.Validators
{
    public class CommentRequestDtoValidator : DtoValidator<CommentRequestDto>
    {
        public CommentRequestDtoValidator()
        {
            RuleFor(request => request.Text)
                .Must(text => !ValidationRules.IsBlank(text))
                .WithMessage("Write a comment before sending.")
                .Must(text => ValidationRules.PresentWithin(text, 240))
                .WithMessage("Comments can be up to 240 characters.");
        }
    }
}
