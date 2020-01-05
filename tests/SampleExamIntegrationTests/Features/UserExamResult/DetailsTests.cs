using SampleExam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.UserExamResult
{
    public class DetailsTests : IntegrationTestBase
    {
        public DetailsTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void UserExamResultDetailsTests()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var testHelper = new UserExamResultTestHelper(client, dbContextFactory);
            var dbContextHelper = new DbContextHelper(dbContextFactory);
            var random = Utils.NewRandom();
            var userExam1 = await httpCallHelper.CreateUserExam(questionCount: random.Next(50, 100));
            var userExam2 = await httpCallHelper.CreateUserExam(questionCount: random.Next(50, 100));
            var userExamNotPublished = await httpCallHelper.CreateUserExam(questionCount: random.Next(50, 100));

            //creatng exam results
            client.Authorize(userExam1.Item1.Token);
            await testHelper.TakeExamAndAssertResult(httpCallHelper, userExam1.Item3.Id, false, true);

            client.Authorize(userExam2.Item1.Token);
            await testHelper.TakeExamAndAssertResult(httpCallHelper, userExam2.Item3.Id, true, true);

            client.Authorize(userExamNotPublished.Item1.Token);
            await testHelper.TakeExamAndAssertResult(httpCallHelper, userExamNotPublished.Item3.Id, false, true);


            //unautorized
            client.Unauthorize();
            await client.GetUnauthorized($"/userexamresults/{userExam1.Item3.Id}");

            client.Authorize(userExam1.Item1.Token);
            //not existing
            client.Authorize(userExam1.Item1.Token);
            await client.GetNotFound($"/userexamresults/{int.MaxValue}");

            //other users
            await client.GetNotFound($"/userexamresults/{userExam2.Item3.Id}");

            //not published
            client.Authorize(userExamNotPublished.Item1.Token);
            await dbContextHelper.SetPublishExamAsync(userExamNotPublished.Item2.Id, false);
            await client.GetNotFound($"/userexamresults/{userExamNotPublished.Item3.Id}");
            await dbContextHelper.SetPublishExamAsync(userExamNotPublished.Item2.Id, true);
            await client.GetUserExamResultSuccesfully($"/userexamresults/{userExamNotPublished.Item3.Id}");

            //normal cases
            client.Authorize(userExam1.Item1.Token);
            await client.GetUserExamResultSuccesfully($"/userexamresults/{userExam1.Item3.Id}");

            client.Authorize(userExam2.Item1.Token);
            await client.GetUserExamResultSuccesfully($"/userexamresults/{userExam2.Item3.Id}");


        }

    }

}