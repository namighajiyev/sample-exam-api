using System;
using System.Linq;
using FluentAssertions;
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

        public static void AssertExamTags(
    SampleExam.Features.Exam.Create.ExamData examData,
    SampleExam.Features.Exam.ExamDTO responseExam,
    SampleExam.Domain.Exam exam
    )
        {
            var examDataTags = examData.Tags.ToArray();
            var responceTags = responseExam.Tags.Select(t => t.Tag).ToArray();
            var examTags = exam.ExamTags.Select(e => e.TagId).ToArray();
            Array.Sort(examDataTags);
            Array.Sort(responceTags);
            Array.Sort(examTags);
            Assert.True(examDataTags.SequenceEqual(responceTags));
            Assert.True(examDataTags.SequenceEqual(examTags));
        }

    }
}