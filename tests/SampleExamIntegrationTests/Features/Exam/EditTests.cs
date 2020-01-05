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
            var dbContextHelper = new DbContextHelper(this.dbContextFactory);
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

            await client.PutUnauthorized(putLink, new Edit.Request() { Exam = examData });

            client.Authorize(loggedUser.Token);
            var responseExam = await client.PutExamSuccesfully(putLink, new Edit.Request() { Exam = examData });


            var exam = dbContextHelper.SelectExamWitTags(examDto.Id);
            var updatedAt1 = exam.UpdatedAt;
            AssertHelper.AssertExam(examData, responseExam, exam);
            AssertHelper.AssertExamTags(examData.Tags.ToArray(), responseExam, exam);

            responseExam = await client.PutExamSuccesfully(putLink, new Edit.Request() { Exam = examData });

            exam = dbContextHelper.SelectExamWitTags(examDto.Id);
            var updatedAt2 = exam.UpdatedAt;
            AssertHelper.AssertExam(examData, responseExam, exam);
            AssertHelper.AssertExamTags(examData.Tags.ToArray(), responseExam, exam);
            Assert.Equal(updatedAt1, updatedAt2);

            responseExam = await client.PutExamSuccesfully(putLink, new Edit.Request() { Exam = new Edit.ExamData() });
            exam = dbContextHelper.SelectExamWitTags(examDto.Id);
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
            var problemDetails = await client.PutBadRequest(putLink, new Edit.Request() { Exam = examData });
            problemDetails.Errors.Should().HaveCount(4);
        }


        [Fact]
        public async void ShouldFailEditOtherUserExamAndPublishedExam()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContextHelper = new DbContextHelper(this.dbContextFactory);
            var tuple = await httpCallHelper.CreateExam();
            var loggedUser1 = tuple.Item1;
            var examDto1 = tuple.Item3;
            tuple = await httpCallHelper.CreateExam();
            var loggedUser2 = tuple.Item1;
            var examDto2 = tuple.Item3;
            var putLink1 = $"/exams/{examDto1.Id}";
            var putLink2 = $"/exams/{examDto2.Id}";
            client.Authorize(loggedUser1.Token);
            await client.PutNotFound(putLink2, new Edit.Request() { Exam = new Edit.ExamData() });
            await client.PutExamSuccesfully(putLink1, new Edit.Request() { Exam = new Edit.ExamData() });

            await dbContextHelper.SetPublishExamAsync(examDto1.Id);
            await client.PutNotFound(putLink1, new Edit.Request() { Exam = new Edit.ExamData() });
        }

    }
}