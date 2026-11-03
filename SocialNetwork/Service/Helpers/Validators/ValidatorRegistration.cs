using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Helpers.Validators
{
    public static class ValidatorRegistration
    {
        public static IServiceCollection AddDtoValidators(this IServiceCollection services)
        {
            return services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        }
    }
}
