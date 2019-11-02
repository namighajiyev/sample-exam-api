using SampleExam;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class ListTests : IntegrationTestBase

    {
        public ListTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {
        }

        [Fact]
        public async void ShouldSelectPublishedExams()
        {

            // Arrange
            var client = httpClientFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/exams");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}