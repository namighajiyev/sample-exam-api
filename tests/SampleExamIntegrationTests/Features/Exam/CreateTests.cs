using System;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class CreateTests : IntegrationTestBase
    {
        public CreateTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFixture dbContextFixture
        ) : base(factory, dbContextFixture)
        {

        }

        [Fact]
        public async void ShouldCreateExamAndFailWhenUnauthorized()
        {
            var client = _factory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFixture.DbContext;
            var tags = SampleExamContextHelper.SeededTags.Select(e => e.TagId).ToArray();
            var loggedUser = (await httpCallHelper.CreateUserAndLogin()).Item4;
            var examData = TestData.Exam.Create.NewExamData(true, false, tags);
            client.Authorize(loggedUser.Token);
            var response = await client.PostAsJsonAsync<Create.Request>("/exams", new Create.Request() { Exam = examData });
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            var exam = dbContext.Exams.Where(e => e.Id == responseExam.Id).Include(e => e.ExamTags).First();
            AssertHelper.AssertExam(examData, responseExam, exam);
            AssertHelper.AssertExamTags(examData, responseExam, exam);
            client.Unauthorize();
            response = await client.PostAsJsonAsync<Create.Request>("/exams", new Create.Request() { Exam = examData });
            response.EnsureUnauthorizedStatusCode();

            //should save without tags
            examData.Tags = null;
            client.Authorize(loggedUser.Token);
            response = await client.PostAsJsonAsync<Create.Request>("/exams", new Create.Request() { Exam = examData });
            response.EnsureSuccessStatusCode();
            envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            responseExam = envelope.Exam;
            exam = dbContext.Exams.Where(e => e.Id == responseExam.Id).Include(e => e.ExamTags).First();
            AssertHelper.AssertExam(examData, responseExam, exam);
            Assert.True(responseExam.Tags.Count == 0);
            Assert.True(exam.ExamTags.Count == 0);
        }

        [Fact]
        public async void ShouldFailCreateExamWithInvalidData()
        {
            var client = _factory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFixture.DbContext;
            var tags = SampleExamContextHelper.SeededTags.Select(e => e.TagId).ToArray();
            var loggedUser = (await httpCallHelper.CreateUserAndLogin()).Item4;
            var examData = TestData.Exam.Create.NewExamData(true, false, tags);
            client.Authorize(loggedUser.Token);
            var response = await client.PostAsJsonAsync<Create.Request>("/exams", new Create.Request() { Exam = examData });
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            Assert.True(responseExam.Id > 0);
            var exam = dbContext.Exams.Where(e => e.Id == responseExam.Id).Include(e => e.ExamTags).First();
            Assert.NotNull(responseExam);
            examData.Title.Should().Be(responseExam.Title).And.Be(exam.Title);
            examData.Description.Should().Be(responseExam.Description).And.Be(exam.Description);
            examData.TimeInMinutes.Should().Be(responseExam.TimeInMinutes).And.Be(exam.TimeInMinutes);
            examData.PassPercentage.Should().Be(responseExam.PassPercentage).And.Be(exam.PassPercentage);
            examData.IsPrivate.Should().Be(responseExam.IsPrivate).And.Be(exam.IsPrivate);
            var examDataTags = examData.Tags.ToArray();
            var responceTags = responseExam.Tags.Select(t => t.Tag).ToArray();
            var examTags = exam.ExamTags.Select(e => e.TagId).ToArray();
            Array.Sort(examDataTags);
            Array.Sort(responceTags);
            Array.Sort(examTags);
            Assert.True(examDataTags.SequenceEqual(responceTags));
            Assert.True(examDataTags.SequenceEqual(examTags));
            client.Unauthorize();
            response = await client.PostAsJsonAsync<Create.Request>("/exams", new Create.Request() { Exam = examData });
            response.EnsureUnauthorizedStatusCode();
        }


    }
}