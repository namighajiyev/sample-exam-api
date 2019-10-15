using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SampleExam.Common;

namespace SampleExam.Features.Question
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
                    Func<TProperty, char> keySelector,
                    string errorCodePrefix)
        {
            return ruleBuilder.NotNull()
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersNotEmpty")
                        .Must(answers => (answers ?? Enumerable.Empty<TProperty>()).Count() >= Constants.QUESTION_ANSWER_MIN_COUNT)
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersMinLength")
                        .Must(answers => (answers ?? Enumerable.Empty<TProperty>()).Count() <= Constants.QUESTION_ANSWER_MAX_COUNT)
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersMaxLength")
                        .Must(answers =>
                        {
                            var answerKeys = (answers ?? Enumerable.Empty<TProperty>())
                            .Select<TProperty, char>(keySelector).ToArray();
                            var answerKeysDistinct = answerKeys.Distinct().ToArray();
                            return answerKeys.Length == answerKeysDistinct.Length;
                        })
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersUniqueKeys")
                                                .Must(answers =>
                        {
                            var answerKeys = (answers ?? Enumerable.Empty<TProperty>())
                            .Select<TProperty, char>(keySelector).ToArray();
                            return answerKeys.All(k => k.ToString().Length == 1);

                        })
                        .WithErrorCode($"{errorCodePrefix}QuestionAnswersKeysMustBeAChar");

        }


        public static IRuleBuilderOptions<T, char> AnswerKey<T, TProperty>(
    this IRuleBuilder<T, char> ruleBuilder,
    string errorCodePrefix)
        {
            return ruleBuilder.NotNull()
                        .WithErrorCode($"{errorCodePrefix}AnswerKeyNotNull")
                        .NotEmpty()
                        .WithErrorCode($"{errorCodePrefix}AnswerKeyNotEmpty")
                        .Must(k => k.ToString().Length == 1)
                        .WithErrorCode($"{errorCodePrefix}AnswerKeyIsChar");
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