using Service.Helpers.DTOs.Common;

namespace Service.Helpers.Validators
{
    public class AdminStatsDtoValidator : DtoValidator<AdminStatsDto>
    {
        public AdminStatsDtoValidator()
        {
            RuleFor(stats => stats.Members).GreaterThanOrEqualTo(0);
            RuleFor(stats => stats.Posts).GreaterThanOrEqualTo(0);
            RuleFor(stats => stats.Reports).GreaterThanOrEqualTo(0);
            RuleFor(stats => stats.Circles).GreaterThanOrEqualTo(0);
        }
    }
}
