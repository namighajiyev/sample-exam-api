using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleExam;
using SampleExam.Features.Auth;
using SampleExam.Features.Exam;
using SampleExam.Features.UserExam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.UserExamResult
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
        public async void UserExamResultCreateTests()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var testHelper = new UserExamResultTestHelper(client, dbContextFactory);
            var dbContextHelper = new DbContextHelper(dbContextFactory);
            var random = Utils.NewRandom();
            var userExam1 = await httpCallHelper.CreateUserExam(questionCount: random.Next(50, 100));
            var userExam2 = await httpCallHelper.CreateUserExam(questionCount: random.Next(50, 100));
            var userExam3 = await httpCallHelper.CreateUserExam(questionCount: random.Next(50, 100));
            var userExam4 = await httpCallHelper.CreateUserExam(questionCount: random.Next(50, 100));

            //unautorized
            client.Unauthorize();
            await client.PostUnauthorized($"/userexamresults/{userExam1.Item3.Id}");

            //not existing
            client.Authorize(userExam1.Item1.Token);
            await client.PostNotFound($"/userexamresults/{int.MaxValue}");

            //other users
            client.Authorize(userExam2.Item1.Token);
            await client.PostNotFound($"/userexamresults/{userExam2.Item3.Id}");

            //not published
            var userExamNotPublished = await httpCallHelper.CreateUserExam();
            await dbContextHelper.SetPublishExamAsync(userExamNotPublished.Item2.Id, false);
            client.Authorize(userExamNotPublished.Item1.Token);
            await client.PostNotFound($"/userexamresults/{userExamNotPublished.Item3.Id}");

            //not ended exam
            var userExamNotEnded = await httpCallHelper.CreateUserExam();
            client.Authorize(userExamNotEnded.Item1.Token);
            await client.PostNotFound($"/userexamresults/{userExamNotEnded.Item3.Id}");
            await dbContextHelper.UpdateUserExamStartDate(userExamNotEnded.Item3.Id, userExamNotEnded.Item2.TimeInMinutes * -2);
            await client.PostUserExamResultSucessfully($"/userexamresults/{userExamNotEnded.Item3.Id}");

            //get existing one
            await client.PostUserExamResultSucessfully($"/userexamresults/{userExamNotEnded.Item3.Id}");

            client.Authorize(userExam1.Item1.Token);
            await TakeExamAndAssertResult(testHelper, httpCallHelper, userExam1.Item3.Id, false, false);

            client.Authorize(userExam2.Item1.Token);
            await TakeExamAndAssertResult(testHelper, httpCallHelper, userExam2.Item3.Id, false, true);

            client.Authorize(userExam3.Item1.Token);
            await TakeExamAndAssertResult(testHelper, httpCallHelper, userExam3.Item3.Id, true, false);

            client.Authorize(userExam4.Item1.Token);
            await TakeExamAndAssertResult(testHelper, httpCallHelper, userExam4.Item3.Id, true, true);

        }


        private async Task TakeExamAndAssertResult(
            UserExamResultTestHelper testHelper,
            HttpCallHelper httpCallHelper,
            int userExamId, bool fail, bool skipSomeQuestions = false)
        {
            var testResult = await testHelper.TakeExam(userExamId, fail, skipSomeQuestions);
            var userExamDto = await httpCallHelper.EndUserExam(userExamId);
            var userExamResult = await httpCallHelper.PostUserExamResult(userExamId);
            Assert.True(userExamResult.IsPassed == !fail);
            Assert.True(userExamResult.RightAnswerCount == testResult.Item1);
            Assert.True(userExamResult.AnsweredQuestionCount == testResult.Item2);
            var passed = ((float)userExamResult.RightAnswerCount / (float)userExamResult.QuestionCount * 100) >= userExamDto.Exam.PassPercentage;
            Assert.True(passed == userExamResult.IsPassed);
            Assert.True(userExamResult.AnsweredQuestionCount == userExamResult.WrongAnswerCount + userExamResult.RightAnswerCount);
            Assert.True(userExamResult.QuestionCount >= userExamResult.AnsweredQuestionCount);
            Assert.True(userExamResult.QuestionCount == userExamResult.AnsweredQuestionCount + userExamResult.NotAnsweredQuestionCount);
        }

    }
}