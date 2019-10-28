using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleExam.Infrastructure.Data;
using SampleExamIntegrationTests.Helpers;

namespace SampleExamIntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {

        public CustomWebApplicationFactory()
        {
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                var serviceProvider = services.BuildServiceProvider();
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                //shift to test db
                var connectionStringTest = config.GetValue<string>(IntegrationTestConstants.CONN_STRING_KEY_TEST);
                config[SampleExam.Common.Constants.CONN_STRING_KEY] = connectionStringTest;
            });

        }
    }
}