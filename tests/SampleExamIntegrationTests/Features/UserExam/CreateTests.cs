using SampleExam;
using SampleExamIntegrationTests.Helpers;
using SampleExamIntegrationTests.Helpers.Data;
using Xunit;

namespace SampleExamIntegrationTests.Features.UserExam
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
        public async void UserExamCreateTests()
        {
            var client = httpClientFactory.CreateClient();
            var data = new QuestionTestData(client);
            var helper = new HttpCallHelper(client);
            client.Unauthorize();
            var u1PublicNotPublishedLink = $"/userexams/{data.u1PublicNotPublished.Item3.Id}";
            var u1PublicPublishedLink = $"/userexams/{data.u1PublicPublished.Item3.Id}";
            var u1PrivateNotPublishedLink = $"/userexams/{data.u1PrivateNotPublished.Item3.Id}";
            var u1PrivatePublishedLink = $"/userexams/{data.u1PrivatePublished.Item3.Id}";

            var u2PublicNotPublishedLink = $"/userexams/{data.u2PublicNotPublished.Item3.Id}";
            var u2PublicPublishedLink = $"/userexams/{data.u2PublicPublished.Item3.Id}";
            var u2PrivateNotPublishedLink = $"/userexams/{data.u2PrivateNotPublished.Item3.Id}";
            var u2PrivatePublishedLink = $"/userexams/{data.u2PrivatePublished.Item3.Id}";
            var linkNoneExisting = $"/userexams/{int.MaxValue}";

            await client.PostUnauthorized(u1PublicNotPublishedLink);

            //login user1
            client.Authorize(data.u1PublicNotPublished.Item1.Token);

            await client.PostNotFound(linkNoneExisting);

            await client.PostNotFound(u1PublicNotPublishedLink);

            var userExamDto = await client.PostUserExamSuccesfully(u1PublicPublishedLink);
            AssertHelper.AsserUserExam(userExamDto);

            await client.PostNotFound(u1PrivateNotPublishedLink);

            userExamDto = await client.PostUserExamSuccesfully(u1PrivatePublishedLink);
            AssertHelper.AsserUserExam(userExamDto);

            await client.PostNotFound(u2PublicNotPublishedLink);

            userExamDto = await client.PostUserExamSuccesfully(u2PublicPublishedLink);
            AssertHelper.AsserUserExam(userExamDto);

            await client.PostNotFound(u2PrivateNotPublishedLink);
            await client.PostNotFound(u2PrivatePublishedLink);

        }

        //create with exam public not published
        //create with exam private not published
        //create with public published
        //create with private published

        //create with other user exam public not published
        //create with other user exam private not published
        //create with other user public published
        //create with other user privaate published

        // create user exam with private exam of another user

    }
}