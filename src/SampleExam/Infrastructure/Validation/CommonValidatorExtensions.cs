using FluentValidation;

namespace SampleExam.Infrastructure.Validation.Common
{
    public static class CommonValidatorExtensions
    {
        public static IRuleBuilderOptions<T, int> Id<T, TProperty>(
             this IRuleBuilder<T, int> ruleBuilder, string errorCodePrefix)
        {
            return ruleBuilder
                        .NotNull()
                        .WithErrorCode($"{errorCodePrefix}IdNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}IdNotEmpty")
                        .GreaterThan(0)
                        .WithErrorCode($"{errorCodePrefix}IdGreaterThan");
        }

    }
}