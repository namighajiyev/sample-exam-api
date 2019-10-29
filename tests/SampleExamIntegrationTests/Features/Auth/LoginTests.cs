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
            DbContextFixture dbContextFixture
        ) : base(factory, dbContextFixture)
        {

        }

        [Fact]
        public async void ShouldLogin()
        {
            var client = _factory.CreateClient();
            var dbContext = this.dbContextFixture.DbContext;
            var userData = TestData.User.Create.NewUserData();
            var response = await client.PostAsJsonAsync<Create.Request>("/users", new Create.Request() { User = userData });
            response.EnsureSuccessStatusCode();
            var loginUser = new Login.UserData() { Email = userData.Email, Password = userData.Password };
            response = await client.PostAsJsonAsync<Login.Request>("/auth/login", new Login.Request() { User = loginUser });
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<LoginUserDTOEnvelope>();
            var user = envelope.User;
            Assert.NotNull(user);
            Assert.NotNull(user.Token);
            Assert.NotNull(user.RefresToken);
        }


        [Fact]
        public async void ShouldFailLogin()
        {
            var client = _factory.CreateClient();
            var dbContext = this.dbContextFixture.DbContext;
            var userData = TestData.User.Create.NewUserData();
            var response = await client.PostAsJsonAsync<Create.Request>("/users", new Create.Request() { User = userData });
            response.EnsureSuccessStatusCode();

            var loginUser = new Login.UserData() { Email = userData.Email, Password = userData.Password + "1" };
            response = await client.PostAsJsonAsync<Login.Request>("/auth/login", new Login.Request() { User = loginUser });
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();

            loginUser = new Login.UserData() { Email = "1" + userData.Email, Password = userData.Password };
            response = await client.PostAsJsonAsync<Login.Request>("/auth/login", new Login.Request() { User = loginUser });
            response.EnsureUnauthorizedStatusCode();
            problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();
        }
    }
}