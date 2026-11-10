using Service.Helpers.DTOs.Report;

namespace Service.Helpers.Validators
{
    public class CreateReportRequestValidator : DtoValidator<CreateReportRequest>
    {
        public CreateReportRequestValidator()
        {
            RuleFor(request => request)
                .Must(request => new[] { request.PostId, request.UserId, request.CommunityId }.Count(id => !ValidationRules.IsBlank(id)) == 1)
                .WithMessage("Choose what to report.");

            RuleFor(request => request.Reason)
                .Must(reason => !ValidationRules.IsBlank(reason))
                .WithMessage("Choose a reason for the report.")
                .Must(reason => ValidationRules.PresentWithin(reason, 120))
                .WithMessage("Reasons can be up to 120 characters.");

            RuleFor(request => request.Details)
                .Must(details => ValidationRules.Within(details, 240))
                .WithMessage("Details can be up to 240 characters.");
        }
    }
}
