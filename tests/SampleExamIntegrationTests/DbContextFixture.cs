using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleExam.Infrastructure.Data;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests
{

    public class DbContextFixture : IDisposable
    {
        private readonly ServiceProvider serviceProvider;

        public DbContextFixture()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var connectionString = config.GetValue<string>(IntegrationTestConstants.CONN_STRING_KEY_TEST);
            services.AddDbContext<SampleExam.Infrastructure.Data.SampleExamContext>(opt => opt.UseNpgsql(connectionString));
            this.serviceProvider = services.BuildServiceProvider();
            DbContext.Database.EnsureCreated();
        }

        public SampleExamContext DbContext
        {
            get
            {
                return serviceProvider.GetRequiredService<SampleExamContext>();
            }

        }

        public void Dispose()
        {
            this.DbContext.Database.EnsureDeleted();
            this.DbContext.Dispose();
        }
    }
}