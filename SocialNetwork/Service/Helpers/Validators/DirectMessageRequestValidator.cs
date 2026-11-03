using Service.Helpers.DTOs.ChatMessage;

namespace Service.Helpers.Validators
{
    public class DirectMessageRequestValidator : DtoValidator<DirectMessageRequest>
    {
        public DirectMessageRequestValidator()
        {
            RuleFor(request => request.Text)
                .Must(text => !ValidationRules.IsBlank(text))
                .WithMessage("Write a message before sending.")
                .Must(text => ValidationRules.PresentWithin(text, 1000))
                .WithMessage("Messages can be up to 1000 characters.");

            RuleFor(request => request.ParticipantId)
                .Must(id => !ValidationRules.IsBlank(id))
                .WithMessage("Choose someone to message.");
        }
    }
}
