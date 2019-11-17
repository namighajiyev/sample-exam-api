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
    public class ListTests : IntegrationTestBase

    {
        public ListTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {
        }

        [Fact]
        public async void ShouldSelectPublishedExams()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();

            //create at least two public and two private exams 
            var tuple = await httpCallHelper.CreateExam();
            var examPublicDto1 = tuple.Item3;
            tuple = await httpCallHelper.CreateExam();
            var examPublicDto2 = tuple.Item3;

            tuple = await httpCallHelper.CreateExam(true, true);
            var examPrivateDto1 = tuple.Item3;
            tuple = await httpCallHelper.CreateExam(true, true);
            var examPrivateDto2 = tuple.Item3;

            var examPublic1 = await dbContext.Exams.FindAsync(examPublicDto1.Id);
            var examPublic2 = await dbContext.Exams.FindAsync(examPublicDto2.Id);

            var examPrivate1 = await dbContext.Exams.FindAsync(examPrivateDto1.Id);
            var examPrivate2 = await dbContext.Exams.FindAsync(examPrivateDto2.Id);

            examPublic1.IsPublished = true;
            examPublic2.IsPublished = true;
            examPrivate1.IsPublished = true;
            examPrivate2.IsPublished = true;
            await dbContext.SaveChangesAsync();

            var getLink = "exams";
            var getLinkIncludeTags = "exams?includeTags=true";
            var getLinkIncludeUser = "exams?includeUser=true";
            var getLinkIncludeTagsAndUser = "exams?includeTags=true&includeUser=true";


            //no user and tags
            var responseExams = await client.GetExamsSuccesfully(getLink);

            foreach (var exam in responseExams)
            {
                Assert.True(exam.IsPublished);
                Assert.False(exam.IsPrivate);
                AssertHelper.AssertNoUserAndNoTagsIncluded(exam);
            }

            //with tags
            responseExams = await client.GetExamsSuccesfully(getLinkIncludeTags);

            foreach (var exam in responseExams)
            {
                Assert.True(exam.IsPublished);
                Assert.False(exam.IsPrivate);
                AssertHelper.AssertOnlyTagsIncluded(exam);
            }

            //with user
            responseExams = await client.GetExamsSuccesfully(getLinkIncludeUser);

            foreach (var exam in responseExams)
            {
                Assert.True(exam.IsPublished);
                Assert.False(exam.IsPrivate);
                AssertHelper.AssertOnlyUserIncluded(exam);
            }

            //with tags and user
            responseExams = await client.GetExamsSuccesfully(getLinkIncludeTagsAndUser);

            foreach (var exam in responseExams)
            {
                Assert.True(exam.IsPublished);
                Assert.False(exam.IsPrivate);
                Assert.NotNull(exam.User);
                if (exam.Id == examPublicDto2.Id || exam.Id == examPublicDto1.Id)
                {
                    Assert.True(exam.Tags.Count > 0);
                }
            }

            //limit, ofset tests...

            //creating some exams
            for (int i = 0; i < 11; i++)
            {
                tuple = await httpCallHelper.CreateExam();
                var examId = tuple.Item3.Id;
                var token = tuple.Item1.Token;
                client.Authorize(token);
                await httpCallHelper.PublishExam(examId);
            }

            var limitOffsetTester = new LimitOffsetTester(client, getLink);
            await limitOffsetTester.DoTest(GetExams);

        }
        private async Task<Tuple<IEnumerable<ExamDTO>, int>> GetExams(HttpClient client, string link)
        {
            var envelope = await client.GetExamsEnvelopeSuccesfully(link);
            return Tuple.Create(envelope.Exams, envelope.ExamCount);
        }
    }
}