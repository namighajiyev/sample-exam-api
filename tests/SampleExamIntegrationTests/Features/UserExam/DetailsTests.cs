using SampleExam;
using SampleExamIntegrationTests.Helpers;
using SampleExamIntegrationTests.Helpers.Data;
using Xunit;

namespace SampleExamIntegrationTests.Features.UserExam
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
        public async void UserExamDetailsTests()
        {
            var client = httpClientFactory.CreateClient();
            var data = new UserExamData(client);
            client.Unauthorize();
            var linkUser1PrivateUserExam = $"/userexams/{data.User1PrivateUserExam.Item3.Id}";
            var linkUser1PublicUserExam = $"/userexams/{data.User1PublicUserExam.Item3.Id}";
            var linkUser1PublicUserExam2 = $"/userexams/{data.User1PublicUserExam2.Item3.Id}";
            var linkUser2PrivateUserExam = $"/userexams/{data.User2PrivateUserExam.Item3.Id}";
            var linkUser2PublicUserExam = $"/userexams/{data.User2PublicUserExam.Item3.Id}";
            var linkUser2PublicUserExam2 = $"/userexams/{data.User2PublicUserExam2.Item3.Id}";

            await client.GetUnauthorized(linkUser1PrivateUserExam);

            client.Authorize(data.User1PrivateUserExam.Item1.Token);

            var userExam = await client.GetUserExamSuccesfully(linkUser1PrivateUserExam);
            AssertHelper.AssertEqual(userExam, data.User1PrivateUserExam.Item3);

            userExam = await client.GetUserExamSuccesfully(linkUser1PublicUserExam);
            AssertHelper.AssertEqual(userExam, data.User1PublicUserExam.Item3);

            userExam = await client.GetUserExamSuccesfully(linkUser1PublicUserExam2);
            AssertHelper.AssertEqual(userExam, data.User1PublicUserExam2.Item3);

            await client.GetNotFound(linkUser2PrivateUserExam);
            await client.GetNotFound(linkUser2PublicUserExam);
            await client.GetNotFound(linkUser2PublicUserExam2);

            client.Authorize(data.User2PrivateUserExam.Item1.Token);

            await client.GetNotFound(linkUser1PrivateUserExam);
            await client.GetNotFound(linkUser1PublicUserExam);
            await client.GetNotFound(linkUser1PublicUserExam2);

            userExam = await client.GetUserExamSuccesfully(linkUser2PrivateUserExam);
            AssertHelper.AssertEqual(userExam, data.User2PrivateUserExam.Item3);

            userExam = await client.GetUserExamSuccesfully(linkUser2PublicUserExam);
            AssertHelper.AssertEqual(userExam, data.User2PublicUserExam.Item3);

            userExam = await client.GetUserExamSuccesfully(linkUser2PublicUserExam2);
            AssertHelper.AssertEqual(userExam, data.User2PublicUserExam2.Item3);

            //include exam
            var includeExams = "?includeExams=true";
            userExam = await client.GetUserExamSuccesfully($"{linkUser2PrivateUserExam}{includeExams}");
            AssertHelper.AssertEqual(userExam, data.User2PrivateUserExam.Item3, true);

            userExam = await client.GetUserExamSuccesfully($"{linkUser2PublicUserExam}{includeExams}");
            AssertHelper.AssertEqual(userExam, data.User2PublicUserExam.Item3, true);

            userExam = await client.GetUserExamSuccesfully($"{linkUser2PublicUserExam2}{includeExams}");
            AssertHelper.AssertEqual(userExam, data.User2PublicUserExam2.Item3, true);

        }

    }
}