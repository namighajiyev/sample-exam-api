using System.Net.Http;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExam.Infrastructure.Errors;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class PublishTests : IntegrationTestBase
    {
        public PublishTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldPublishExam()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();
            var tuple = await httpCallHelper.CreateExam();
            var loggedUser1 = tuple.Item1;
            var examDto1 = tuple.Item3;
            tuple = await httpCallHelper.CreateExam();
            var loggedUser2 = tuple.Item1;
            var examDto2 = tuple.Item3;
            var link1 = $"exams/publish/{examDto1.Id}";
            var link2 = $"exams/publish/{examDto2.Id}";
            //unauthorized
            client.PutUnauthorized(link1, null);

            //other user's exam.
            client.Authorize(loggedUser1.Token);
            client.PutNotFound(link2, null);

            //already published
            var exam1 = await dbContext.Exams.FindAsync(examDto1.Id);
            exam1.IsPublished = true;
            await dbContext.SaveChangesAsync();
            client.PutNotFound(link1, null);

            //sucess
            client.Authorize(loggedUser2.Token);
            var responseExam = client.PutExamSuccesfully(link2, null);

            //check success
            var exam2 = await dbContext.Exams.FindAsync(examDto2.Id);
            Assert.True(exam2.IsPublished);
            Assert.True(responseExam.IsPublished);

        }
    }
}