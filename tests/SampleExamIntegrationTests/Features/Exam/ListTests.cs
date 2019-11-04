using System.Linq;
using System.Net.Http;
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
            var response = await client.GetAsync(getLink);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamsDTOEnvelope>();
            var responseExams = envelope.Exams;

            foreach (var exam in responseExams)
            {
                Assert.True(exam.IsPublished);
                Assert.False(exam.IsPrivate);
                Assert.Null(exam.User);
                Assert.True(exam.Tags.Count == 0);
            }

            //with tags
            response = await client.GetAsync(getLinkIncludeTags);
            response.EnsureSuccessStatusCode();
            envelope = await response.Content.ReadAsAsync<ExamsDTOEnvelope>();
            responseExams = envelope.Exams;

            foreach (var exam in responseExams)
            {
                Assert.True(exam.IsPublished);
                Assert.False(exam.IsPrivate);
                Assert.Null(exam.User);
                Assert.True(exam.Tags.Count > 0);
            }

            //with user
            response = await client.GetAsync(getLinkIncludeUser);
            response.EnsureSuccessStatusCode();
            envelope = await response.Content.ReadAsAsync<ExamsDTOEnvelope>();
            responseExams = envelope.Exams;

            foreach (var exam in responseExams)
            {
                Assert.True(exam.IsPublished);
                Assert.False(exam.IsPrivate);
                Assert.NotNull(exam.User);
                Assert.True(exam.Tags.Count == 0);
            }

            //with tags and user
            response = await client.GetAsync(getLinkIncludeTagsAndUser);
            response.EnsureSuccessStatusCode();
            envelope = await response.Content.ReadAsAsync<ExamsDTOEnvelope>();
            responseExams = envelope.Exams;

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

            var limit = 2;
            var offset = 0;
            var count = 0;
            var haveExams = false;
            do
            {
                var link = $"{getLink}?limit={limit}&offset={offset}";
                response = await client.GetAsync(link);
                response.EnsureSuccessStatusCode();
                envelope = await response.Content.ReadAsAsync<ExamsDTOEnvelope>();
                responseExams = envelope.Exams;
                var responseCount = responseExams.Count();
                count = envelope.ExamCount;
                offset += limit;
                haveExams = offset < count;
                Assert.True(responseCount == limit || (responseCount < limit && !haveExams));
            }
            while (haveExams);


        }
    }
}