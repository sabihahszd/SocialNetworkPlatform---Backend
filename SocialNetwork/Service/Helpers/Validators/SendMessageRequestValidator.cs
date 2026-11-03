using Service.Helpers.DTOs.ChatMessage;

namespace Service.Helpers.Validators
{
    public class SendMessageRequestValidator : DtoValidator<SendMessageRequest>
    {
        public SendMessageRequestValidator()
        {
            RuleFor(request => request.Text)
                .Must(text => !ValidationRules.IsBlank(text))
                .WithMessage("Write a message before sending.")
                .Must(text => ValidationRules.PresentWithin(text, 1000))
                .WithMessage("Messages can be up to 1000 characters.");
        }
    }
}
