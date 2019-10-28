using SampleExam;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class ListTests : IntegrationTestBase

    {
        public ListTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFixture dbContextFixture) : base(factory, dbContextFixture)
        {
        }

        [Fact]
        public async void ShouldSelectPublishedExams()
        {

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/exams");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}