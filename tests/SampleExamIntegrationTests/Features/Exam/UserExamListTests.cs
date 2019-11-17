using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class UserExamListTests : IntegrationTestBase

    {
        public UserExamListTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {
        }



        [Fact]
        public async void ShouldGetUserExams()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            //var dbContext = this.dbContextFactory.CreateDbContext();

            //create 4 public user exam out of 2 are published
            var tuple = await httpCallHelper.CreateExam();
            var user = tuple.Item1;
            await httpCallHelper.CreateExam(true, false, null, user);
            await httpCallHelper.CreatePublishedExam(true, false, null, user);
            await httpCallHelper.CreatePublishedExam(true, false, null, user);

            //create 4 private user exam out of 2 are 
            await httpCallHelper.CreateExam(true, true, null, user);
            await httpCallHelper.CreateExam(true, true, null, user);
            await httpCallHelper.CreatePublishedExam(true, true, null, user);
            await httpCallHelper.CreatePublishedExam(true, true, null, user);

            var linkAll1 = "exams/user/exams";
            var linkAll2 = "exams/user/exams?privateType=all&publishType=all";
            var linkAllPrivates = "exams/user/exams?privateType=private";
            var linkAllPublics = "exams/user/exams?privateType=public";
            var linkAllPublished = "exams/user/exams?publishType=published";
            var linkAllNotPublished = "exams/user/exams?publishType=notPublished";
            await client.GetUnauthorized(linkAll1);
            client.Authorize(user.Token);
            //all
            var exams = await client.GetExamsSuccesfully(linkAll1);
            Assert.True(exams.Count() == 8);
            //all2
            exams = await client.GetExamsSuccesfully(linkAll2);
            Assert.True(exams.Count() == 8);
            //privates
            exams = await client.GetExamsSuccesfully(linkAllPrivates);
            Assert.True(exams.Count() == 4);
            //publics
            exams = await client.GetExamsSuccesfully(linkAllPublics);
            Assert.True(exams.Count() == 4);
            //published
            exams = await client.GetExamsSuccesfully(linkAllPublished);
            Assert.True(exams.Count() == 4);
            //not published
            exams = await client.GetExamsSuccesfully(linkAllNotPublished);
            Assert.True(exams.Count() == 4);

            //creating more user exams ...

            for (int i = 0; i < 5; i++)
            {
                var isPrivate = i % 2 == 0;
                await httpCallHelper.CreateExam(true, isPrivate, null, user);
            }

            //authorize after create exam unauthirized
            client.Authorize(user.Token);

            var limitOffsetTester = new LimitOffsetTester(client, linkAll1);
            await limitOffsetTester.DoTest(GetExams);
        }

        private async Task<Tuple<IEnumerable<ExamDTO>, int>> GetExams(HttpClient client, string link)
        {
            var envelope = await client.GetExamsEnvelopeSuccesfully(link);
            return Tuple.Create(envelope.Exams, envelope.ExamCount);
        }
    }
}