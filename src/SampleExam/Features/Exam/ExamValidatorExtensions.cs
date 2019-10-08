using FluentValidation;
using SampleExam.Common;

namespace SampleExam.Features.Exam
{
    public static class ExamValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> ExamTitle<T, TProperty>(
            this IRuleBuilder<T, string> ruleBuilder,
            string errorCodePrefix)
        {
            return ruleBuilder.NotNull()
                        .WithErrorCode($"{errorCodePrefix}ExamTitleNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}ExamTitleNotEmpty")
                        .MaximumLength(Constants.EXAM_TITLE_LEN)
                        .WithErrorCode($"{errorCodePrefix}ExamTitleMaximumLength");
        }

        public static IRuleBuilderOptions<T, string> ExamDescription<T, TProperty>(
            this IRuleBuilder<T, string> ruleBuilder,
            string errorCodePrefix)
        {
            return ruleBuilder
                        .NotNull()
                        .WithErrorCode($"{errorCodePrefix}ExamDescriptionNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}ExamDescriptionNotEmpty")
                        .MaximumLength(Constants.EXAM_DESCRIPTIION_LEN)
                        .WithErrorCode($"{errorCodePrefix}DescriptionMaximumLength");
        }


        public static IRuleBuilderOptions<T, int> ExamTimeInMinutes<T, TProperty>(
            this IRuleBuilder<T, int> ruleBuilder,
            string errorCodePrefix)
        {
            return ruleBuilder
                        .NotNull()
                        .WithErrorCode($"{errorCodePrefix}ExamTimeInMinutesNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}ExamTimeInMinutesNotEmpty")
                        .LessThanOrEqualTo(Constants.EXAM_TIME_IN_MINUTES_MAX)
                        .WithErrorCode($"{errorCodePrefix}ExamTimeInMinutesLessThanOrEqualTo")
                        .GreaterThanOrEqualTo(Constants.EXAM_TIME_IN_MINUTES_MIN)
                        .WithErrorCode($"{errorCodePrefix}ExamTimeInMinutesGreaterThanOrEqualTo");
        }

        public static IRuleBuilderOptions<T, int> ExamPassPercentage<T, TProperty>(
            this IRuleBuilder<T, int> ruleBuilder,
            string errorCodePrefix)
        {
            return ruleBuilder
                        .NotNull()
                        .WithErrorCode($"{errorCodePrefix}ExamPassPercentageNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}ExamPassPercentageNotEmpty")
                        .LessThanOrEqualTo(Constants.EXAM_PASS_PERCENTAGE_MAX)
                        .WithErrorCode($"{errorCodePrefix}ExamPassPercentageLessThanOrEqualTo")
                        .GreaterThanOrEqualTo(Constants.EXAM_PASS_PERCENTAGE_MIN)
                        .WithErrorCode($"{errorCodePrefix}ExamPassPercentageGreaterThanOrEqualTo");
        }

        public static IRuleBuilderOptions<T, bool> ExamIsPrivate<T, TProperty>(
              this IRuleBuilder<T, bool> ruleBuilder,
              string errorCodePrefix)
        {
            return ruleBuilder
                        .NotNull()
                        .WithErrorCode($"{errorCodePrefix}ExamIsPrivateNotNull");
        }

    }
}
