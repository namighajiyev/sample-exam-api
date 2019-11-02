using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleExam.Infrastructure.Data;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests
{

    public class TestsInitializer : IDisposable
    {
        private SampleExamContext dbContext;

        public TestsInitializer()
        {
            BeforeAllTests();
        }

        private void BeforeAllTests()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var connectionString = config.GetValue<string>(IntegrationTestConstants.CONN_STRING_KEY_TEST);
            services.AddDbContext<SampleExam.Infrastructure.Data.SampleExamContext>(opt => opt.UseNpgsql(connectionString));
            var serviceProvider = services.BuildServiceProvider();
            this.dbContext = serviceProvider.GetRequiredService<SampleExamContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            SampleExamContextHelper.SeedContext(dbContext);
        }

        public void Dispose()
        {
            AfterAllTests();
        }

        private void AfterAllTests()
        {
            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Dispose();
        }
    }
}