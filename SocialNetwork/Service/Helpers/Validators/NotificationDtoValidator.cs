using Service.Helpers.DTOs.Notification;

namespace Service.Helpers.Validators
{
    public class NotificationDtoValidator : DtoValidator<NotificationDto>
    {
        public NotificationDtoValidator()
        {
            RuleFor(notification => notification.Id).NotEmpty();
            RuleFor(notification => notification.Type)
                .Must(ValidationRules.IsNotificationType)
                .WithMessage("Choose a notification type.");
            RuleFor(notification => notification.Status)
                .Must(ValidationRules.IsRequestStatus)
                .WithMessage("Choose pending, accepted, declined, or done.");
        }
    }
}
