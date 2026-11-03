using FluentValidation;

namespace Service.Helpers.Validators
{
    public abstract class DtoValidator<T> : AbstractValidator<T>
    {
        protected DtoValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;
        }
    }
}
