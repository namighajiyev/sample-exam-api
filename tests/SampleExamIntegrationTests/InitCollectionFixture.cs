using Xunit;

namespace SampleExamIntegrationTests
{
    [CollectionDefinition("Init collection")]
    public class InitCollectionFixture : ICollectionFixture<TestsInitializer>
    {

    }
}