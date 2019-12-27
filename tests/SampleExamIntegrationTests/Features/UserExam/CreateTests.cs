using SampleExam;
using Xunit;

namespace SampleExamIntegrationTests.Features.UserExam
{
    public class CreateTests : IntegrationTestBase
    {
        public CreateTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        // [Fact]
        // public async void ShouldCreateTests()
        // {
        // }
        //unauthorized
        // nonexisting

        //create with exam public not published
        //create with exam private not published
        //create with public published
        //create with private published

        //create with other user exam public not published
        //create with other user exam private not published
        //create with other user public published
        //create with other user privaate published

        // create user exam with private exam of another user

    }
}