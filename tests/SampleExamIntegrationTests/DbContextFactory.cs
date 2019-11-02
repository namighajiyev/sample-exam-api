using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleExam.Infrastructure.Data;
using SampleExamIntegrationTests.Helpers;

namespace SampleExamIntegrationTests
{
    public class DbContextFactory
    {
        public SampleExamContext CreateDbContext()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var connectionString = config.GetValue<string>(IntegrationTestConstants.CONN_STRING_KEY_TEST);
            services.AddDbContext<SampleExam.Infrastructure.Data.SampleExamContext>(opt => opt.UseNpgsql(connectionString));
            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<SampleExamContext>();
            return dbContext;
        }
    }
}