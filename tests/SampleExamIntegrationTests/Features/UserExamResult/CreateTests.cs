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
            await testHelper.TakeExamAndAssertResult(httpCallHelper, userExam1.Item3.Id, false, false);

            //other users
            client.Authorize(userExam2.Item1.Token);
            await client.PostNotFound($"/userexamresults/{userExam2.Item3.Id}");

            client.Authorize(userExam2.Item1.Token);
            await testHelper.TakeExamAndAssertResult(httpCallHelper, userExam2.Item3.Id, false, true);

            client.Authorize(userExam3.Item1.Token);
            await testHelper.TakeExamAndAssertResult(httpCallHelper, userExam3.Item3.Id, true, false);

            client.Authorize(userExam4.Item1.Token);
            await testHelper.TakeExamAndAssertResult(httpCallHelper, userExam4.Item3.Id, true, true);

        }



    }
}