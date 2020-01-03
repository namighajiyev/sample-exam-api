using System;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExam.Infrastructure.Errors;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class CreateTests : IntegrationTestBase
    {
        public CreateTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldCreateExamAndFailWhenUnauthorized()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContextHelper = new DbContextHelper(this.dbContextFactory);
            var tags = SampleExamContextHelper.SeededTags.Select(e => e.TagId).ToArray();
            var loggedUser = (await httpCallHelper.CreateUserAndLogin()).Item4;
            var examData = TestData.Exam.Create.NewExamData(true, false, tags);
            client.Authorize(loggedUser.Token);
            var responseExam = await client.PostExamSuccesfully("/exams", new Create.Request() { Exam = examData });
            var exam = dbContextHelper.SelectExamWitTags(responseExam.Id);
            AssertHelper.AssertExam(examData, responseExam, exam);
            AssertHelper.AssertExamTags(examData.Tags.ToArray(), responseExam, exam);
            client.Unauthorize();
            await client.PostUnauthorized("/exams", new Create.Request() { Exam = examData });

            //should save without tags
            examData.Tags = null;
            client.Authorize(loggedUser.Token);
            responseExam = await client.PostExamSuccesfully("/exams", new Create.Request() { Exam = examData });

            exam = dbContextHelper.SelectExamWitTags(responseExam.Id);
            AssertHelper.AssertExam(examData, responseExam, exam);
            Assert.True(responseExam.Tags.Count == 0);
            Assert.True(exam.ExamTags.Count == 0);
        }

        [Fact]
        public async void ShouldFailCreateExamWithInvalidData()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var loggedUser = (await httpCallHelper.CreateUserAndLogin()).Item4;
            client.Authorize(loggedUser.Token);
            var problemDetails = await client.PostBadRequest("/exams", new Create.Request() { Exam = new Create.ExamData() });
            problemDetails.Errors.Should().HaveCount(4);
        }


    }
}