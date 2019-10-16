using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SampleExam.Common;
 


namespace SampleExamUnitTests.SampleExamContext
{
    public class SampleExamContextTests
    {
        private SampleExam.Infrastructure.Data.SampleExamContext context;

        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var connectionString = config.GetValue<string>(Constants.CONN_STRING_KEY);
            services.AddDbContext<SampleExam.Infrastructure.Data.SampleExamContext>(opt => opt.UseNpgsql(connectionString));
            var sp = services.BuildServiceProvider();
            this.context = sp.GetRequiredService<SampleExam.Infrastructure.Data.SampleExamContext>();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }

        [Test]
        public void ShouldSelectUserCount()
        {
            var users = this.context.Users.CountAsync().Result;
        }

        [Test]
        public void ShouldSelectExamCount()
        {
            var exams = this.context.Exams.CountAsync().Result;
        }
    }
}