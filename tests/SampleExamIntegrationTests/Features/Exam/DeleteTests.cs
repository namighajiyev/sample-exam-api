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
            var response = await client.DeleteAsync(link1);
            response.EnsureUnauthorizedStatusCode();

            //not this users exam
            client.Authorize(loggedUser1.Token);
            response = await client.DeleteAsync(link2);
            response.EnsureNotFoundStatusCode();

            //not existing
            response = await client.DeleteAsync(linkNotExists);
            response.EnsureNotFoundStatusCode();


            //success 
            response = await client.DeleteAsync(link1);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            Assert.Equal(responseExam.Id, examDto1.Id);
            exam1 = dbContext.Exams.Where(e => e.Id == examDto1.Id).FirstOrDefault();
            Assert.Null(exam1);

            exam1 = dbContext.Exams.Where(e => e.Id == examDto1.Id)
            .IgnoreQueryFilters()
            .First();
            //     AssertHelper.AssertExamDeleted(exam1);



            // var exam1 = await dbContext.Exams.FindAsync(examDto1.Id);
            // exam1.IsPublished = true;
            // await dbContext.SaveChangesAsync();
            // response = await client.PutAsync(link1, null);
            // response.EnsureNotFoundStatusCode();
            // client.Authorize(loggedUser2.Token);
            // response = await client.PutAsync(link2, null);
            // response.EnsureSuccessStatusCode();
            // var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            // var responseExam = envelope.Exam;
            // var exam2 = await dbContext.Exams.FindAsync(examDto2.Id);
            // Assert.True(exam2.IsPublished);
            // Assert.True(responseExam.IsPublished);
        }

    }
}