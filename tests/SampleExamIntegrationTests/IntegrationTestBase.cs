using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace SampleExamIntegrationTests
{
    public class IntegrationTestBase :
    IClassFixture<CustomWebApplicationFactory<SampleExam.Startup>>
    {
        protected readonly CustomWebApplicationFactory<SampleExam.Startup> _factory;

        public IntegrationTestBase(CustomWebApplicationFactory<SampleExam.Startup> factory)
        {
            _factory = factory;
        }

    }
}