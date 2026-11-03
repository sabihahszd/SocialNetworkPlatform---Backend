using Service.Helpers.DTOs.ChatMessage;

namespace Service.Helpers.Validators
{
    public class ChatMessageDtoValidator : DtoValidator<ChatMessageDto>
    {
        public ChatMessageDtoValidator()
        {
            RuleFor(message => message.Id).NotEmpty();
            RuleFor(message => message.AuthorId).NotEmpty();
            RuleFor(message => message.Text)
                .Must(text => ValidationRules.PresentWithin(text, 1000))
                .WithMessage("Messages can be up to 1000 characters.");
        }
    }
}
