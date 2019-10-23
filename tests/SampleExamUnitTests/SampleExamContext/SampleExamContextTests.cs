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

        public SampleExamContextHelper contextHelper { get; private set; }

        [SetUp]
        public void Setup()
        {
            var serviceProvider = UnitTestHelper.GetServiceProvider();
            this.context = serviceProvider.GetRequiredService<SampleExam.Infrastructure.Data.SampleExamContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            this.contextHelper = new SampleExamContextHelper(context, serviceProvider);
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

        [Test]
        public void dddd()
        {
            var user = contextHelper.AddNewUser();
            var exam = contextHelper.AddNewExam(user.Id);
            var userExam = contextHelper.AddUserExam(user.Id, exam.Id);
        }
    }
}