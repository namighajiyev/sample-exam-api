using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleExam.Domain;

namespace SampleExamUnitTests
{
    public static class UnitTestHelper
    {
        public static ServiceProvider GetServiceProvider()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var connectionString = config.GetValue<string>(UnitTestConstants.CONN_STRING_KEY);
            services.AddDbContext<SampleExam.Infrastructure.Data.SampleExamContext>(opt => opt.UseNpgsql(connectionString));
            services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
            return services.BuildServiceProvider();
        }

    }

}