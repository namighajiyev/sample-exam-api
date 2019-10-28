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
            DbContextFixture dbContextFixture
        ) : base(factory, dbContextFixture)
        {

        }

        [Fact]
        public async void ShouldCreateUser()
        {
            var client = _factory.CreateClient();
            var dbContext = this.dbContextFixture.DbContext;
            var uniqueEmail = $"{Guid.NewGuid().ToString().Replace("-", String.Empty)}@example.com";
            var userData = new Create.UserData()
            {
                Firstname = "Namig",
                Lastname = "Hajiyev",
                Middlename = "Zakir",
                GenderId = 1,
                Dob = new DateTime(1986, 04, 07),
                Email = uniqueEmail,
                Password = "2aEvJPCF",
                ConfirmPassword = "2aEvJPCF"
            };

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
        }

        [Fact]
        public async void ShouldNotCreateUserWithInvalidUserData()
        {
            var client = _factory.CreateClient();
            var dbContext = this.dbContextFixture.DbContext;
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
        }

    }
}