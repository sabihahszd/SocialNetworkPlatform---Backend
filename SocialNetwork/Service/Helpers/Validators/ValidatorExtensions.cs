using FluentValidation;

namespace Service.Helpers.Validators
{
    public static class ValidatorExtensions
    {
        public static async Task<string?> FirstErrorAsync<T>(this IValidator<T> validator, T model, params string[] ruleSets)
        {
            var result = ruleSets.Length == 0
                ? await validator.ValidateAsync(model)
                : await validator.ValidateAsync(model, options => options.IncludeRuleSets(ruleSets));

            return result.IsValid ? null : result.Errors[0].ErrorMessage;
        }
    }
}
