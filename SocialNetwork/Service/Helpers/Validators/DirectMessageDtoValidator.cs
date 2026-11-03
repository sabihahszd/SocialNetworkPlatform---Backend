using Service.Helpers.DTOs.ChatMessage;

namespace Service.Helpers.Validators
{
    public class DirectMessageDtoValidator : DtoValidator<DirectMessageDto>
    {
        public DirectMessageDtoValidator()
        {
            RuleFor(message => message.ThreadId).NotEmpty();
            RuleFor(message => message.ParticipantId).NotEmpty();
            RuleFor(message => message.Message).NotNull().SetValidator(new ChatMessageDtoValidator());
        }
    }
}
