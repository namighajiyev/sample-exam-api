using System.Linq;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class DeleteTests : IntegrationTestBase
    {
        public DeleteTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldDeleteExam()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();
            var tuple = await httpCallHelper.CreateExam();
            var loggedUser1 = tuple.Item1;
            var examDto1 = tuple.Item3;
            tuple = await httpCallHelper.CreateExam();
            var loggedUser2 = tuple.Item1;
            var examDto2 = tuple.Item3;
            var link1 = $"exams/{examDto1.Id}";
            var link2 = $"exams/{examDto2.Id}";
            var linkNotExists = $"exams/{int.MaxValue}";

            var exam1 = await dbContext.Exams.FindAsync(examDto1.Id);
            var exam2 = await dbContext.Exams.FindAsync(examDto2.Id);
            AssertHelper.AssertExamNotDeleted(exam1);
            AssertHelper.AssertExamNotDeleted(exam2);

            //unauthorized
            client.DeleteUnauthorized(link1);

            //not this users exam
            client.Authorize(loggedUser1.Token);
            client.DeleteNotFound(link2);

            //not existing
            client.DeleteNotFound(linkNotExists);

            //success 
            var responseExam = client.DeleteExamSucessfully(link1);
            Assert.Equal(responseExam.Id, examDto1.Id);
            exam1 = dbContext.Exams.Where(e => e.Id == examDto1.Id).FirstOrDefault();
            Assert.Null(exam1);

            //new db context to work around reload problem
            dbContext = this.dbContextFactory.CreateDbContext();
            exam1 = dbContext.Exams.Where(e => e.Id == examDto1.Id)
            .IgnoreQueryFilters()
            .First();
            AssertHelper.AssertExamDeleted(exam1);
        }

    }
}