using Service.Helpers.DTOs.Report;

namespace Service.Helpers.Validators
{
    public class ReportDtoValidator : DtoValidator<ReportDto>
    {
        public ReportDtoValidator()
        {
            RuleFor(report => report.Id).NotEmpty();
            RuleFor(report => report.ReporterId).NotEmpty();
            RuleFor(report => report.Reason)
                .Must(reason => ValidationRules.PresentWithin(reason, 400))
                .WithMessage("Reasons can be up to 400 characters.");
            RuleFor(report => report.Status)
                .Must(ValidationRules.IsReportStatus)
                .WithMessage("Choose open, dismissed, or removed.");
        }
    }
}
