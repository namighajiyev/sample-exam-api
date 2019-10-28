using Xunit;

namespace SampleExamIntegrationTests
{
    [CollectionDefinition("Database collection")]
    public class DbContextCollectionFixture : ICollectionFixture<DbContextFixture>
    {

    }
}