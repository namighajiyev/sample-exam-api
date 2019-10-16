using System;
using FluentValidation;
using SampleExam.Common;
 
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Validation;

namespace SampleExam.Infrastructure.Validation.User
{
    public static class UserValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> UserFirstname<T, TProperty>(
        this IRuleBuilder<T, string> ruleBuilder,
        string errorCodePrefix)
        {
            return ruleBuilder
                .NotNull()
                .WithErrorCode($"{errorCodePrefix}UserFirstnameNotNull")
                .NotEmpty()
                .WithErrorCode($"{errorCodePrefix}UserFirstnameNotEmpty")
                .MaximumLength(Constants.USER_FIRSTNAME_LEN)
                .WithErrorCode($"{errorCodePrefix}UserFirstnameMaximumLength");
        }

        public static IRuleBuilderOptions<T, string> UserMiddlename<T, TProperty>(
        this IRuleBuilder<T, string> ruleBuilder,
        string errorCodePrefix)
        {
            return ruleBuilder
                .MaximumLength(Constants.USER_MIDDLENAME_LEN)
                .WithErrorCode($"{errorCodePrefix}UserMiddlenameMaximumLength");
        }

        public static IRuleBuilderOptions<T, string> UserLastname<T, TProperty>(
        this IRuleBuilder<T, string> ruleBuilder,
        string errorCodePrefix)
        {
            return ruleBuilder
                .NotNull()
                .WithErrorCode($"{errorCodePrefix}UserLastnameNotNull")
                .NotEmpty()
                .WithErrorCode($"{errorCodePrefix}UserLastnameNotEmpty")
                .MaximumLength(Constants.USER_LASTNAME_LEN)
                .WithErrorCode($"{errorCodePrefix}UserLastnameMaximumLength");
        }

        public static IRuleBuilderOptions<T, int> UserGenderId<T, TProperty>(
        this IRuleBuilder<T, int> ruleBuilder,
        string errorCodePrefix)
        {
            return ruleBuilder
                .NotNull()
                .WithErrorCode($"{errorCodePrefix}UserGenderIdNotNull")
                .NotEmpty()
                .WithErrorCode($"{errorCodePrefix}UserGenderIdNotEmpty")
                .Must(id => id == SeedData.Genders.Male.Id || id == SeedData.Genders.Female.Id)
                .WithMessage($"User GenderId must be either {SeedData.Genders.Male.Id} ({SeedData.Genders.Male.Name}) or {SeedData.Genders.Female.Id} ({SeedData.Genders.Female.Name})")
                .WithErrorCode($"{errorCodePrefix}UserGenderIdMustBeMaleOrFemale");
        }


        public static IRuleBuilderOptions<T, DateTime> UserDob<T, TProperty>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        string errorCodePrefix)
        {
            return ruleBuilder
                .NotNull()
                .WithErrorCode($"{errorCodePrefix}UserDobNotNull")
                .NotEmpty()
                .WithErrorCode($"{errorCodePrefix}UserDobNotEmpty")
                .LessThan(DateTime.UtcNow.Date)
                .WithMessage($"User DOB must be less than {DateTime.UtcNow.Date}")
                .WithErrorCode($"{errorCodePrefix}UserDobLessThanUtcNowDate");
        }

        public static IRuleBuilderOptions<T, string> UserEmail<T, TProperty>(
        this IRuleBuilder<T, string> ruleBuilder,
        string errorCodePrefix, bool isUnique = true)
        {
            var builder = ruleBuilder
                .NotNull()
                .WithErrorCode($"{errorCodePrefix}UserEmailNotNull")
                .NotEmpty()
                .WithErrorCode($"{errorCodePrefix}UserEmailNotEmpty")
                .EmailAddress()
                .WithErrorCode($"{errorCodePrefix}UserEmailEmailAddress")
                .MaximumLength(Constants.USER_EMAIL_LEN)
                .WithErrorCode($"{errorCodePrefix}UserEmailMaximumLength");

            if (isUnique)
            {
                builder = builder
                .UniqueEmail()
                .WithErrorCode($"{errorCodePrefix}UserEmailUniqueEmail");
            }

            return builder;
        }

        public static IRuleBuilderOptions<T, string> UserPassword<T, TProperty>(
        this IRuleBuilder<T, string> ruleBuilder,
        string errorCodePrefix)
        {
            return ruleBuilder
                .NotNull()
                .WithErrorCode($"{errorCodePrefix}UserPasswordNotNull")
                .NotEmpty()
                .WithErrorCode($"{errorCodePrefix}UserPasswordNotEmpty")
                .MinimumLength(Constants.PASSWORD_MIN_LENGTH)
                .WithErrorCode($"{errorCodePrefix}UserPasswordMinimumLength")
                .Matches(Constants.ASCII_PRINTABLE_CHARS_REGEX)
                .WithErrorCode($"{errorCodePrefix}UserPasswordMatchesAsciiPrintableChars")
                .WithMessage("Password must contain only printable ASCII characters")
                .NotMatches(Constants.NOT_BEGINING_OR_ENDING_WITH_ASCII_SPACE_REGEX)
                .WithErrorCode($"{errorCodePrefix}UserPasswordMatchesNotBeginsOrEndsWithAsciiSpace")
                .WithMessage("Your password cannot start or end with a blank space")
                .StrongPassword()
                .WithErrorCode($"{errorCodePrefix}UserPasswordStrongPassword");
        }

        public static IRuleBuilderOptions<T, string> UserConfirmPassword<T, TProperty>(
        this IRuleBuilder<T, string> ruleBuilder,
        Func<T, string> passwordFunc,
        string errorCodePrefix)
        {
            return ruleBuilder
                .NotNull()
                .WithErrorCode($"{errorCodePrefix}UserConfirmPasswordNotNull")
                .NotEmpty()
                .WithErrorCode($"{errorCodePrefix}UserConfirmPasswordNotEmpty")
                .Equal(x => passwordFunc(x))
                .WithErrorCode($"{errorCodePrefix}UserConfirmPasswordEqualPassword")
                .WithMessage("Passwords do not match");
        }
    }
}