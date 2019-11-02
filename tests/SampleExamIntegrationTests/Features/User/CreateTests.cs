using System.Net.Http;
using Xunit;
using SampleExam.Features.User;
using System;
using System.Linq;
using FluentAssertions;
using SampleExamIntegrationTests.Helpers;
using SampleExam.Infrastructure.Errors;
using SampleExam;

namespace SampleExamIntegrationTests.Features.User
{
    public class CreateTests : IntegrationTestBase
    {
        public CreateTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldCreateUserAndFailWithTheSameEmail()
        {
            var client = httpClientFactory.CreateClient();
            var dbContext = this.dbContextFactory.CreateDbContext();

            var userData = TestData.User.Create.NewUserData();
            var response = await client.PostAsJsonAsync<Create.Request>("/users", new Create.Request() { User = userData });
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<UserDTOEnvelope>();
            var responseUser = envelope.User;
            var user = dbContext.Users.Where(e => e.Email == userData.Email).First();

            userData.Firstname.Should().Be(responseUser.Firstname).And.Be(user.Firstname);
            userData.Lastname.Should().Be(responseUser.Lastname).And.Be(user.Lastname);
            userData.Middlename.Should().Be(responseUser.Middlename).And.Be(user.Middlename);
            userData.GenderId.Should().Be(responseUser.GenderId).And.Be(user.GenderId);
            userData.Email.Should().Be(responseUser.Email).And.Be(user.Email);
            userData.Password.Should().NotBe(user.Password);

            response = await client.PostAsJsonAsync<Create.Request>("/users", new Create.Request() { User = userData });
            response.EnsureBadRequestStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();

            var hasUniqueEmailError = problemDetails.Errors.Any(kv => kv.Value.Any(e => e.Code == "CreateUserEmailUniqueEmail"));
            Assert.True(hasUniqueEmailError);
        }

        [Fact]
        public async void ShouldNotCreateUserWithInvalidUserData()
        {
            var client = httpClientFactory.CreateClient();
            var dbContext = this.dbContextFactory.CreateDbContext();
            var userData = new Create.UserData()
            {
                Firstname = "",
                Lastname = "",
                Middlename = "",
                GenderId = 0,
                Dob = new DateTime(DateTime.Now.Year + 1, 04, 07),
                Email = "namiqexample.com",
                Password = "aaaa",
                ConfirmPassword = "bbbb"
            };

            var response = await client.PostAsJsonAsync<Create.Request>("/users", new Create.Request() { User = userData });
            response.EnsureBadRequestStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();
            problemDetails.Errors.Should().HaveCount(7);

            response = await client.PostAsJsonAsync<Create.Request>("/users", new Create.Request());
            problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();

            var hasNotNullError = problemDetails.Errors.Any(kv => kv.Value.Any(e => e.Code == "CreateUserNotNull"));
            Assert.True(hasNotNullError);
        }

    }
}