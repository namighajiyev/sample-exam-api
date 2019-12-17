using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SampleExam.Common;

namespace SampleExam.Infrastructure.Validation.Question
{
    public static class QuestionValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> QuestionText<T, TProperty>(
            this IRuleBuilder<T, string> ruleBuilder,
            string errorCodePrefix)
        {
            return ruleBuilder.NotNull()
                        .WithErrorCode($"{errorCodePrefix}QuestionTextNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}QuestionTextNotEmpty")
                        .MaximumLength(Constants.QUESTION_TEXT_LEN)
                        .WithErrorCode($"{errorCodePrefix}QuestionTextMaximumLength");
        }


        public static IRuleBuilderOptions<T, IEnumerable<TProperty>> QuestionAnswers<T, TProperty>(
                    this IRuleBuilder<T, IEnumerable<TProperty>> ruleBuilder,
                    Func<TProperty, bool> isRightSelector,
                    string errorCodePrefix)
        {
            return ruleBuilder.NotNull()
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersNotEmpty")
                        .Must(answers => (answers ?? Enumerable.Empty<TProperty>()).Count() >= Constants.QUESTION_ANSWER_MIN_COUNT)
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersMinLength")
                        .WithMessage($"Question must have at least {Constants.QUESTION_ANSWER_MIN_COUNT} answer options")
                        .Must(answers => (answers ?? Enumerable.Empty<TProperty>()).Count() <= Constants.QUESTION_ANSWER_MAX_COUNT)
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersMaxLength")
                        .WithMessage($"Question must have at most {Constants.QUESTION_ANSWER_MAX_COUNT} answer options")
                        .Must(answers =>
                        {
                            var isRights = (answers ?? Enumerable.Empty<TProperty>())
                            .Select<TProperty, bool>(isRightSelector).ToArray();
                            return isRights.Any(k => k);
                        })
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersAtLeastOneAnswerMustBeRight")
                        .WithMessage("At least one answer option  must be right")
                        .Must(answers =>
                        {
                            var isRights = (answers ?? Enumerable.Empty<TProperty>())
                            .Select<TProperty, bool>(isRightSelector).ToArray();
                            return isRights.Any(k => !k);
                        })
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersAtLeastOneKeyMustBeWrong")
                        .WithMessage("At least one answer option  must be wrong");

        }


        public static IRuleBuilderOptions<T, IEnumerable<TProperty>> QuestionTypeRadio<T, TProperty>(
                    this IRuleBuilder<T, IEnumerable<TProperty>> ruleBuilder,
                    Func<TProperty, bool> isRightSelector,
                    string errorCodePrefix)
        {
            return ruleBuilder.NotNull()
                        .Must(answers =>
                        {
                            return (answers ?? Enumerable.Empty<TProperty>())
                            .Where(isRightSelector).Count() == 1;
                        })
                        .WithErrorCode($"{errorCodePrefix}QuestionTypeRadioRightAnswerCountMustBeOne")
                        .WithMessage("Radio type question must be exactly one right answer");
        }

        public static IRuleBuilderOptions<T, IEnumerable<TProperty>> QuestionTypeCheckbox<T, TProperty>(
            this IRuleBuilder<T, IEnumerable<TProperty>> ruleBuilder,
            Func<TProperty, bool> isRightSelector,
            string errorCodePrefix)
        {
            return ruleBuilder.NotNull()
                        .Must(answers =>
                        {
                            return (answers ?? Enumerable.Empty<TProperty>())
                           .Where(isRightSelector).Count() > 1;
                        })
                        .WithErrorCode($"{errorCodePrefix}QuestionTypeRadioRightAnswerCountMustBeOne")
                        .WithMessage("Checkbox type question must have more than one right answers");
        }


        public static IRuleBuilderOptions<T, string> AnswerText<T, TProperty>(
            this IRuleBuilder<T, string> ruleBuilder,
            string errorCodePrefix)
        {
            return ruleBuilder.NotNull()
                        .WithErrorCode($"{errorCodePrefix}AnswerTextNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}AnswerTextNotEmpty")
                        .MaximumLength(Constants.ANSWEROPTION_TEXT_LEN)
                        .WithErrorCode($"{errorCodePrefix}AnswerTextMaximumLength");
        }


        public static IRuleBuilderOptions<T, bool> AnswerIsRight<T, TProperty>(
              this IRuleBuilder<T, bool> ruleBuilder,
              string errorCodePrefix)
        {
            return ruleBuilder
                        .NotNull()
                        .WithErrorCode($"{errorCodePrefix}AnswerIsRightNotNull");
        }

    }
}