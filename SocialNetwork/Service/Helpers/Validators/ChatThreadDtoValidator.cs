using Service.Helpers.DTOs.ChatThread;

namespace Service.Helpers.Validators
{
    public class ChatThreadDtoValidator : DtoValidator<ChatThreadDto>
    {
        public ChatThreadDtoValidator()
        {
            RuleFor(thread => thread.Id).NotEmpty();
            RuleFor(thread => thread.ParticipantId).NotEmpty();
            RuleFor(thread => thread.Messages).NotNull();
            RuleForEach(thread => thread.Messages).SetValidator(new ChatMessageDtoValidator());
        }
    }
}
