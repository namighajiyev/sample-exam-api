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
            var dbContextHelper = new DbContextHelper(this.dbContextFactory);
            var tuple = await httpCallHelper.CreateExam();
            var loggedUser1 = tuple.Item1;
            var examDto1 = tuple.Item3;
            tuple = await httpCallHelper.CreateExam();
            var loggedUser2 = tuple.Item1;
            var examDto2 = tuple.Item3;
            var link1 = $"exams/{examDto1.Id}";
            var link2 = $"exams/{examDto2.Id}";
            var linkNotExists = $"exams/{int.MaxValue}";

            var exam1 = await dbContextHelper.FindExamAsync(examDto1.Id);
            var exam2 = await dbContextHelper.FindExamAsync(examDto2.Id);
            AssertHelper.AssertExamNotDeleted(exam1);
            AssertHelper.AssertExamNotDeleted(exam2);

            //unauthorized
            await client.DeleteUnauthorized(link1);

            //not this users exam
            client.Authorize(loggedUser1.Token);
            await client.DeleteNotFound(link2);

            //not existing
            await client.DeleteNotFound(linkNotExists);

            //success 
            var responseExam = await client.DeleteExamSucessfully(link1);
            Assert.Equal(responseExam.Id, examDto1.Id);
            exam1 = dbContextHelper.SelectExamFirstOrDefault(examDto1.Id);
            Assert.Null(exam1);

            exam1 = dbContextHelper.SelectExamIgnoreQueryFiltersTakeFirst(examDto1.Id);
            AssertHelper.AssertExamDeleted(exam1);
        }

    }
}