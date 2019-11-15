using System.Net.Http;
using SampleExam;
using SampleExam.Features.Auth;
using SampleExam.Features.User;
using SampleExam.Infrastructure.Errors;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Auth
{
    public class LoginTests : IntegrationTestBase
    {
        public LoginTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldLogin()
        {
            var client = httpClientFactory.CreateClient();
            var dbContext = this.dbContextFactory.CreateDbContext();
            var userData = TestData.User.Create.NewUserData();
            await client.PostSucessfully("/users", new Create.Request() { User = userData });
            var loginUser = new Login.UserData() { Email = userData.Email, Password = userData.Password };
            var user = await client.PostLoginSucessfully("/auth/login", new Login.Request() { User = loginUser });
            Assert.NotNull(user);
            Assert.NotNull(user.Token);
            Assert.NotNull(user.RefresToken);
        }


        [Fact]
        public async void ShouldFailLogin()
        {
            var client = httpClientFactory.CreateClient();
            var dbContext = this.dbContextFactory.CreateDbContext();
            var userData = TestData.User.Create.NewUserData();
            await client.PostSucessfully("/users", new Create.Request() { User = userData });

            var loginUser = new Login.UserData() { Email = userData.Email, Password = userData.Password + "1" };
            var problemDetails = await client.PostUnauthorized("/auth/login", new Login.Request() { User = loginUser });

            loginUser = new Login.UserData() { Email = "1" + userData.Email, Password = userData.Password };
            problemDetails = await client.PostUnauthorized("/auth/login", new Login.Request() { User = loginUser });
        }
    }
}