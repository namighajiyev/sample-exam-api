using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SampleExamIntegrationTests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<SampleExam.Startup>>
    {
        private readonly WebApplicationFactory<SampleExam.Startup> _factory;

        public UnitTest1(WebApplicationFactory<SampleExam.Startup> factory)
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
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            // Assert.Equal("text/html; charset=utf-8",
            //     response.Content.Headers.ContentType.ToString());
        }
    }
}
