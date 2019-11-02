using Xunit;
namespace SampleExamIntegrationTests
{
    [Collection("Init collection")]
    public class IntegrationTestBase :
    IClassFixture<CustomWebApplicationFactory<SampleExam.Startup>>,
    IClassFixture<DbContextFactory>
    {
        protected readonly CustomWebApplicationFactory<SampleExam.Startup> httpClientFactory;
        protected readonly DbContextFactory dbContextFactory;

        public IntegrationTestBase(
            CustomWebApplicationFactory<SampleExam.Startup> httpClientFactory,
            DbContextFactory dbContextFactory
        )
        {
            this.httpClientFactory = httpClientFactory;
            this.dbContextFactory = dbContextFactory;
        }



    }
}