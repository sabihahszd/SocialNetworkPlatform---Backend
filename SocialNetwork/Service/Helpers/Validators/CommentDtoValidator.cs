using Service.Helpers.DTOs.Comment;

namespace Service.Helpers.Validators
{
    public class CommentDtoValidator : DtoValidator<CommentDto>
    {
        public CommentDtoValidator()
        {
            RuleFor(comment => comment.Id).NotEmpty();
            RuleFor(comment => comment.AuthorId).NotEmpty();
            RuleFor(comment => comment.Text)
                .Must(text => ValidationRules.PresentWithin(text, 240))
                .WithMessage("Comments can be up to 240 characters.");
            RuleFor(comment => comment.Replies).NotNull();
            RuleForEach(comment => comment.Replies).SetValidator(this);
        }
    }
}
