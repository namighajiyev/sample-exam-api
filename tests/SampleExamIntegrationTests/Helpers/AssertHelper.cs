using System;
using System.Linq;
using FluentAssertions;
using SampleExam.Domain;
using SampleExam.Features.Answer;
using SampleExam.Features.Exam;
using SampleExam.Features.Question;
using SampleExam.Features.QuestionAnswer;
using SampleExam.Features.UserExam;
using Xunit;

namespace SampleExamIntegrationTests.Helpers
{
    public static class AssertHelper
    {
        public static void AssertExam(
            SampleExam.Features.Exam.Create.ExamData examData,
            SampleExam.Features.Exam.ExamDTO responseExam,
            SampleExam.Domain.Exam exam
            )
        {
            Assert.True(responseExam.Id > 0);
            Assert.NotNull(responseExam);
            examData.Title.Should().Be(responseExam.Title).And.Be(exam.Title);
            examData.Description.Should().Be(responseExam.Description).And.Be(exam.Description);
            examData.TimeInMinutes.Should().Be(responseExam.TimeInMinutes).And.Be(exam.TimeInMinutes);
            examData.PassPercentage.Should().Be(responseExam.PassPercentage).And.Be(exam.PassPercentage);
            examData.IsPrivate.Should().Be(responseExam.IsPrivate).And.Be(exam.IsPrivate);
            (examData.Tags ?? new string[] { }).Count().Should().Be(responseExam.Tags.Count).And.Be(exam.ExamTags.Count);
        }

        public static void AssertExam(
    SampleExam.Features.Exam.Edit.ExamData examData,
    SampleExam.Features.Exam.ExamDTO responseExam,
    SampleExam.Domain.Exam exam
    )
        {
            Assert.True(responseExam.Id > 0);
            Assert.NotNull(responseExam);
            examData.Title.Should().Be(responseExam.Title).And.Be(exam.Title);
            examData.Description.Should().Be(responseExam.Description).And.Be(exam.Description);
            examData.TimeInMinutes.Should().Be(responseExam.TimeInMinutes).And.Be(exam.TimeInMinutes);
            examData.PassPercentage.Should().Be(responseExam.PassPercentage).And.Be(exam.PassPercentage);
            examData.IsPrivate.Should().Be(responseExam.IsPrivate).And.Be(exam.IsPrivate);
            (examData.Tags ?? new string[] { }).Count().Should().Be(responseExam.Tags.Count).And.Be(exam.ExamTags.Count);
        }

        internal static void AssertEqual(UserExamDTO userExamResponse, UserExamDTO userExam2, bool includeExams = false)
        {
            Assert.Equal(userExamResponse.EndedAt, userExam2.EndedAt);
            Assert.Equal(userExamResponse.ExamId, userExam2.ExamId);
            Assert.Equal(userExamResponse.Id, userExam2.Id);
            Assert.Equal(userExamResponse.StartedtedAt, userExam2.StartedtedAt);
            Assert.Equal(userExamResponse.UserId, userExam2.UserId);
            if (includeExams)
            {
                Assert.True(userExamResponse.Exam != null);
            }
            else
            {
                Assert.True(userExamResponse.Exam == null);
            }

        }

        internal static void AssertUpdated(UserExamDTO userExamResponce, UserExamDTO userExamRequest)
        {
            Assert.False(userExamRequest.EndedAt.HasValue);
            Assert.True(userExamResponce.EndedAt.HasValue);
            Assert.True(userExamResponce.EndedAt < DateTime.UtcNow);
            Assert.True(userExamResponce.EndedAt > DateTime.MinValue);
            Assert.Equal(userExamResponce.ExamId, userExamRequest.ExamId);
            Assert.Equal(userExamResponce.Id, userExamRequest.Id);
            Assert.Equal(userExamResponce.StartedtedAt, userExamRequest.StartedtedAt);
            Assert.Equal(userExamResponce.UserId, userExamRequest.UserId);
        }

        internal static void AssertExamNotDeleted(Exam exam)
        {
            Assert.NotNull(exam);
            Assert.False(exam.IsDeleted);
            Assert.Null(exam.DeletedAt);
        }

        internal static void AssertExamDeleted(Exam exam)
        {
            Assert.NotNull(exam);
            Assert.True(exam.IsDeleted);
            Assert.NotNull(exam.DeletedAt);
        }

        public static void AssertExamTags(
            string[] examDataTags,
    SampleExam.Features.Exam.ExamDTO responseExam,
    SampleExam.Domain.Exam exam
    )
        {
            // var examDataTags = examData.Tags.ToArray();
            var responceTags = responseExam.Tags.Select(t => t.Tag).ToArray();
            var examTags = exam.ExamTags.Select(e => e.TagId).ToArray();
            Array.Sort(examDataTags);
            Array.Sort(responceTags);
            Array.Sort(examTags);
            Assert.True(examDataTags.SequenceEqual(responceTags));
            Assert.True(examDataTags.SequenceEqual(examTags));
        }

        internal static void AssertEqual(QuestionAnswerDTO questionAnswer, CreateOrUpdate.UserExamQuestionAnswerData userExamQuestionAnswer)
        {
            Assert.NotNull(questionAnswer.CreatedAt);
            Assert.NotNull(questionAnswer.UpdatedAt);
            Assert.Equal(questionAnswer.QuestionId, userExamQuestionAnswer.QuestionId);
            Assert.Equal(questionAnswer.UserExamId, userExamQuestionAnswer.UserExamId);
            Assert.True(questionAnswer.AnswerOptions.Select(e => e.AnswerOptionId).OrderBy(e => e).ToArray()
            .SequenceEqual(userExamQuestionAnswer.AnswerOptionIds.OrderBy(e => e).ToArray()));
        }

        public static void AssertUserAndTagsIncluded(ExamDTO responseExam)
        {
            Assert.NotNull(responseExam.User);
            Assert.True(responseExam.Tags.Count > 0);
        }

        public static void AssertOnlyTagsIncluded(ExamDTO responseExam)
        {
            Assert.Null(responseExam.User);
            Assert.True(responseExam.Tags.Count > 0);
        }

        public static void AssertOnlyUserIncluded(ExamDTO responseExam)
        {
            Assert.NotNull(responseExam.User);
            Assert.True(responseExam.Tags.Count == 0);
        }

        public static void AssertNoUserAndNoTagsIncluded(ExamDTO responseExam)
        {
            Assert.Null(responseExam.User);
            Assert.True(responseExam.Tags.Count == 0);
        }

        internal static void AssertEqual(QuestionDTO x, QuestionDTO y)
        {
            var comparer = new QuestionDtoComparer();
            var equal = comparer.Equals(x, y);
            Assert.True(equal);
        }

        internal static void AssertEqual(QuestionAnswerDTO x, QuestionAnswerDTO y)
        {
            var comparer = new QuestionAnswerDtoComparer();
            var equal = comparer.Equals(x, y);
            Assert.True(equal);
        }

        internal static void AssertNotEqual(QuestionAnswerDTO x, QuestionAnswerDTO y)
        {
            var comparer = new QuestionAnswerDtoComparer();
            var equal = comparer.Equals(x, y);
            Assert.False(equal);
        }

        internal static void AssertNotEqual(QuestionDTO x, QuestionDTO y)
        {
            var comparer = new QuestionDtoComparer();
            var equal = comparer.Equals(x, y);
            Assert.False(equal);
        }

        internal static void AssertEqual(AnswerOptionDTO answerOption, SampleExam.Features.Question.Edit.AnswerData answer)
        {
            if (answer.Id.HasValue)
            {
                Assert.True(answer.Id == answerOption.Id);
            }

            Assert.True(answer.IsRight == answerOption.IsRight && answer.Text == answerOption.Text);
        }

        internal static void AsserUserExam(UserExamDTO userExam)
        {
            Assert.True(userExam.Id > 0);
            Assert.True(userExam.ExamId > 0);
            Assert.True(userExam.StartedtedAt < DateTime.UtcNow);
            Assert.True(userExam.StartedtedAt > DateTime.MinValue);
            Assert.True(userExam.UserId > 0);
            Assert.True(!userExam.EndedAt.HasValue);
        }

    }

}