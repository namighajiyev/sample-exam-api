using System.Net.Http;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
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
        public async void ShouldGetExamDetails()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();

            var tuple = await httpCallHelper.CreateExam();
            var examPublicDto = tuple.Item3;

            tuple = await httpCallHelper.CreateExam(true, true);
            var examPrivateDto = tuple.Item3;

            var getExamLink = $"exams/exam/{examPublicDto.Id}";
            var getPrivateExamLink = $"exams/exam/{examPrivateDto.Id}";

            //not published
            var response = await client.GetAsync(getExamLink);
            response.EnsureNotFoundStatusCode();
            response = await client.GetAsync(getPrivateExamLink);
            response.EnsureNotFoundStatusCode();

            var examPublic = await dbContext.Exams.FindAsync(examPublicDto.Id);
            var examPrivate = await dbContext.Exams.FindAsync(examPrivateDto.Id);
            examPublic.IsPublished = true;
            examPrivate.IsPublished = true;
            await dbContext.SaveChangesAsync();

            //public and  published
            response = await client.GetAsync(getExamLink);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;

            //private and  published
            response = await client.GetAsync(getPrivateExamLink);
            response.EnsureNotFoundStatusCode();

        }

    }
}