using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public static IRuleBuilderOptions<T, IEnumerable<TProperty>> NotEmptyEnumerable<T, TProperty>(
         this IRuleBuilder<T, IEnumerable<TProperty>> ruleBuilder, string errorCodePrefix)
        {
            return ruleBuilder
                        .NotNull()
                        .WithErrorCode($"{errorCodePrefix}EnumerableNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}EnumerableNotEmpty")
                        .Must(en => en.Count() > 0)
                        .WithErrorCode($"{errorCodePrefix}EnumerableCountGreaterThanZero");
        }

    }
}