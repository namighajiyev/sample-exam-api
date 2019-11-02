using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Utils;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class EditTests : IntegrationTestBase
    {
        public EditTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldEditExam()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();
            var tuple = await httpCallHelper.CreateExam();
            var loggedUser = tuple.Item1;
            var examDto = tuple.Item3;
            var putLink = $"/exams/{examDto.Id}";
            var newTags = new List<string>();
            newTags.Add(examDto.Tags.First().Tag);
            newTags.AddRange(SampleExamContextHelper.SeededTags.Select(e => e.TagId).ToArray());
            var uniqueString = Guid.NewGuid().ToGuidString();
            newTags.AddRange(new string[] { $"{uniqueString}_Tag1", $"{uniqueString}_Tag2" });

            var examData = new Edit.ExamData()
            {
                Title = $"{examDto.Title}_2",

                Description = $"{examDto.Description}_2",

                TimeInMinutes = examDto.TimeInMinutes + 1,

                PassPercentage = examDto.PassPercentage + 1,

                IsPrivate = !examDto.IsPrivate,

                Tags = newTags
            };

            var response = await client.PutAsJsonAsync<Edit.Request>(putLink, new Edit.Request() { Exam = examData });
            response.EnsureUnauthorizedStatusCode();
            client.Authorize(loggedUser.Token);
            response = await client.PutAsJsonAsync<Edit.Request>(putLink, new Edit.Request() { Exam = examData });
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            var exam = dbContext.Exams.Where(e => e.Id == examDto.Id).Include(e => e.ExamTags).First();
            var updatedAt1 = exam.UpdatedAt;
            AssertHelper.AssertExam(examData, responseExam, exam);
            AssertHelper.AssertExamTags(examData.Tags.ToArray(), responseExam, exam);

            response = await client.PutAsJsonAsync<Edit.Request>(putLink, new Edit.Request() { Exam = examData });
            response.EnsureSuccessStatusCode();
            envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            responseExam = envelope.Exam;
            exam = dbContext.Exams.Where(e => e.Id == examDto.Id).Include(e => e.ExamTags).First();
            var updatedAt2 = exam.UpdatedAt;
            AssertHelper.AssertExam(examData, responseExam, exam);
            AssertHelper.AssertExamTags(examData.Tags.ToArray(), responseExam, exam);
            Assert.Equal(updatedAt1, updatedAt2);


            response = await client.PutAsJsonAsync<Edit.Request>(putLink, new Edit.Request() { Exam = new Edit.ExamData() });
            response.EnsureSuccessStatusCode();
            envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            responseExam = envelope.Exam;
            exam = dbContext.Exams.Where(e => e.Id == examDto.Id).Include(e => e.ExamTags).First();
            var updatedAt3 = exam.UpdatedAt;
            AssertHelper.AssertExam(examData, responseExam, exam);
            AssertHelper.AssertExamTags(examData.Tags.ToArray(), responseExam, exam);
            Assert.Equal(updatedAt2, updatedAt3);
        }


        [Fact]
        public async void ShouldFailEditExamWithInvalidData()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();
            var tuple = await httpCallHelper.CreateExam();
            var loggedUser = tuple.Item1;
            var examDto = tuple.Item3;
            var putLink = $"/exams/{examDto.Id}";

            var examData = new Edit.ExamData()
            {
                Title = "",

                Description = "",

                TimeInMinutes = 0,

                PassPercentage = 0,

                IsPrivate = !examDto.IsPrivate,

                Tags = new string[] { }
            };
            client.Authorize(loggedUser.Token);
            var response = await client.PutAsJsonAsync<Edit.Request>(putLink, new Edit.Request() { Exam = examData });
            response.EnsureBadRequestStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();
            problemDetails.Errors.Should().HaveCount(4);
        }
    }
}