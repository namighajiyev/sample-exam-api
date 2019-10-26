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
        private ServiceProvider serviceProvider;

        public CustomWebApplicationFactory()
        {
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                this.serviceProvider = services.BuildServiceProvider();
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                //shift to test db
                var connectionStringTest = config.GetValue<string>(IntegrationTestConstants.CONN_STRING_KEY_TEST);
                config[SampleExam.Common.Constants.CONN_STRING_KEY] = connectionStringTest;

                services.AddDbContext<SampleExamContext>(opt => opt.UseNpgsql(connectionStringTest));
                this.serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<SampleExamContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

            });

        }

        public void DeletAndCreateDb()
        {
            var context = GetSampleExamContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public SampleExamContext GetSampleExamContext()
        {
            return this.serviceProvider.GetRequiredService<SampleExamContext>();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                GetSampleExamContext().Dispose();
                this.serviceProvider.Dispose();
            }
        }

    }
}