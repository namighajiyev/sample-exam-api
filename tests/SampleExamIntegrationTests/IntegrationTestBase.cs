using Xunit;
namespace SampleExamIntegrationTests
{
    [Collection("Database collection")]
    public class IntegrationTestBase :
    IClassFixture<CustomWebApplicationFactory<SampleExam.Startup>>
    {
        protected readonly CustomWebApplicationFactory<SampleExam.Startup> _factory;
        protected readonly DbContextFixture dbContextFixture;

        public IntegrationTestBase(
            CustomWebApplicationFactory<SampleExam.Startup> factory,
            DbContextFixture dbContextFixture
        )
        {
            _factory = factory;
            this.dbContextFixture = dbContextFixture;
        }

    }
}