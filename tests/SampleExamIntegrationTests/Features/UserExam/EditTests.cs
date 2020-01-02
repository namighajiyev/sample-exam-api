using SampleExam;
using SampleExamIntegrationTests.Helpers;
using SampleExamIntegrationTests.Helpers.Data;
using Xunit;

namespace SampleExamIntegrationTests.Features.UserExam
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
        public async void UserExamEditTests()
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

            await client.PutUnauthorized(linkUser1PrivateUserExam);

            client.Authorize(data.User1PrivateUserExam.Item1.Token);

            var userExam = await client.PutUserExamSuccesfully(linkUser1PrivateUserExam);
            AssertHelper.AssertUpdated(userExam, data.User1PrivateUserExam.Item3);

            userExam = await client.PutUserExamSuccesfully(linkUser1PublicUserExam);
            AssertHelper.AssertUpdated(userExam, data.User1PublicUserExam.Item3);

            userExam = await client.PutUserExamSuccesfully(linkUser1PublicUserExam2);
            AssertHelper.AssertUpdated(userExam, data.User1PublicUserExam2.Item3);

            await client.PutNotFound(linkUser2PrivateUserExam);
            await client.PutNotFound(linkUser2PublicUserExam);
            await client.PutNotFound(linkUser2PublicUserExam2);

            client.Authorize(data.User2PrivateUserExam.Item1.Token);

            await client.PutNotFound(linkUser1PrivateUserExam);
            await client.PutNotFound(linkUser1PublicUserExam);
            await client.PutNotFound(linkUser1PublicUserExam2);

            userExam = await client.PutUserExamSuccesfully(linkUser2PrivateUserExam);
            AssertHelper.AssertUpdated(userExam, data.User2PrivateUserExam.Item3);

            userExam = await client.PutUserExamSuccesfully(linkUser2PublicUserExam);
            AssertHelper.AssertUpdated(userExam, data.User2PublicUserExam.Item3);

            userExam = await client.PutUserExamSuccesfully(linkUser2PublicUserExam2);
            AssertHelper.AssertUpdated(userExam, data.User2PublicUserExam2.Item3);

            //allready finished
            await client.PutBadRequest(linkUser2PrivateUserExam);
            await client.PutBadRequest(linkUser2PublicUserExam);
            await client.PutBadRequest(linkUser2PublicUserExam2);
        }

    }
}