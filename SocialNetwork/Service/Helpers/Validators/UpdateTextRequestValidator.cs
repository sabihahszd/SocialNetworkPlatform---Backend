using Service.Helpers.DTOs.Common;

namespace Service.Helpers.Validators
{
    public class UpdateTextRequestValidator : DtoValidator<UpdateTextRequest>
    {
        public const string Post = "Post";
        public const string Comment = "Comment";
        public const string Message = "Message";

        public UpdateTextRequestValidator()
        {
            RuleSet(Post, () =>
            {
                RuleFor(request => request.Text)
                    .Must(text => !ValidationRules.IsBlank(text))
                    .WithMessage("Write something before sharing.")
                    .Must(text => ValidationRules.PresentWithin(text, 1000))
                    .WithMessage("Posts can be up to 1000 characters.");
            });

            RuleSet(Comment, () =>
            {
                RuleFor(request => request.Text)
                    .Must(text => !ValidationRules.IsBlank(text))
                    .WithMessage("Write a comment before sending.")
                    .Must(text => ValidationRules.PresentWithin(text, 240))
                    .WithMessage("Comments can be up to 240 characters.");
            });

            RuleSet(Message, () =>
            {
                RuleFor(request => request.Text)
                    .Must(text => !ValidationRules.IsBlank(text))
                    .WithMessage("Write a message before sending.")
                    .Must(text => ValidationRules.PresentWithin(text, 1000))
                    .WithMessage("Messages can be up to 1000 characters.");
            });
        }
    }
}
