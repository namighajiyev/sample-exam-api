using SampleExam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class UserExamDetailTests : IntegrationTestBase
    {
        public UserExamDetailTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }


        [Fact]
        public async void ShouldGetUserExamDetails()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();

            var tuple = await httpCallHelper.CreateExam();
            var examPublicDto1 = tuple.Item3;
            var loginUserDto1 = tuple.Item1;

            tuple = await httpCallHelper.CreateExam(true, true, null, loginUserDto1);
            var examPrivateDto1 = tuple.Item3;

            tuple = await httpCallHelper.CreateExam();
            var examPublicDto2 = tuple.Item3;
            var loginUserDto2 = tuple.Item1;

            var getExamLink1 = $"exams/user/exam/{examPublicDto1.Id}";
            var getExamLink1Private = $"exams/user/exam/{examPrivateDto1.Id}";
            var getExamLink2 = $"exams/user/exam/{examPublicDto2.Id}";

            //unauthorized

            var response = await client.GetAsync(getExamLink1);
            response.EnsureUnauthorizedStatusCode();

            client.Authorize(loginUserDto1.Token);

            //other users and not published
            response = await client.GetAsync(getExamLink2);
            response.EnsureNotFoundStatusCode();

            //not published
            response = await client.GetAsync(getExamLink1);
            response.EnsureNotFoundStatusCode();
            response = await client.GetAsync(getExamLink1Private);
            response.EnsureNotFoundStatusCode();


            //publish exams
            await httpCallHelper.PublishExam(examPublicDto1.Id);
            await httpCallHelper.PublishExam(examPrivateDto1.Id);

            client.Authorize(loginUserDto2.Token);
            await httpCallHelper.PublishExam(examPublicDto2.Id);
            client.Authorize(loginUserDto1.Token);

        }


    }
}