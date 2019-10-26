using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SampleExam.Infrastructure.Data;
using Xunit;

namespace SampleExamIntegrationTests
{
    public class IntegrationTest1 :
    IClassFixture<CustomWebApplicationFactory<SampleExam.Startup>>

    {
        private readonly CustomWebApplicationFactory<SampleExam.Startup> _factory;

        public IntegrationTest1(CustomWebApplicationFactory<SampleExam.Startup> factory)
        {
            _factory = factory;

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
