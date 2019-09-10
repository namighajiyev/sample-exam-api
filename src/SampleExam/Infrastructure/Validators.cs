using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Resources;
using FluentValidation.Validators;

namespace SampleExam.Infrastructure
{
    public static class Validators
    {
        public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new StrongPasswordValidator("Password is not strong enough"));
        }

        public static IRuleBuilderOptions<T, string> NotMatches<T>(this IRuleBuilder<T, string> ruleBuilder, string expression)
        {
            return ruleBuilder.SetValidator(new NegativeRegularExpressionValidator(expression));
        }
    }

    internal class StrongPasswordValidator : PropertyValidator
    {
        public StrongPasswordValidator(string errorMessage) : base(errorMessage)
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            string password = context.PropertyValue?.ToString();
            if (string.IsNullOrWhiteSpace(password)) return false;
            var weakPasswords = new List<string>() { "123456" };
            var isWeakPassword = weakPasswords.Contains(password);
            return !isWeakPassword;
        }
    }

    internal class NegativeRegularExpressionValidator : RegularExpressionValidator
    {
        public NegativeRegularExpressionValidator(string expression) : base(expression)
        {
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            return !base.IsValid(context);
        }
    }
}