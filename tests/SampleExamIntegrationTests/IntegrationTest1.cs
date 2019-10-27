using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SampleExam;
using SampleExam.Infrastructure.Data;
using Xunit;

namespace SampleExamIntegrationTests
{
    public class IntegrationTest1 : IntegrationTestBase

    {
        public IntegrationTest1(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async void Test1()
        {

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/exams");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
