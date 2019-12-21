using System.Linq;
using SampleExam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Question
{
    public class ListAuthQuestionsTest : IntegrationTestBase
    {
        public ListAuthQuestionsTest(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }
        [Fact]
        public async void AllListAuthQuestionsTest()
        {
            var client = httpClientFactory.CreateClient();
            var data = new QuestionTestData(client);
            var helper = new HttpCallHelper(client);
            client.Unauthorize();
            var u1PublicNotPublishedLink = $"/questions/private?examId={data.u1PublicNotPublished.Item3.Id}";
            var u1PublicPublishedLink = $"/questions/private?examId={data.u1PublicPublished.Item3.Id}";
            var u1PrivateNotPublishedLink = $"/questions/private?examId={data.u1PrivateNotPublished.Item3.Id}";
            var u1PrivatePublishedLink = $"/questions/private?examId={data.u1PrivatePublished.Item3.Id}";

            var u2PublicNotPublishedLink = $"/questions/private?examId={data.u2PublicNotPublished.Item3.Id}";
            var u2PublicPublishedLink = $"/questions/private?examId={data.u2PublicPublished.Item3.Id}";
            var u2PrivateNotPublishedLink = $"/questions/private?examId={data.u2PrivateNotPublished.Item3.Id}";
            var u2PrivatePublishedLink = $"/questions/private?examId={data.u2PrivatePublished.Item3.Id}";
            var linkNoneExisting = $"/questions/private?examId={int.MaxValue}";

            //login user1
            client.Authorize(data.u1PublicNotPublished.Item1.Token);

            await client.GetNotFound(linkNoneExisting);

            var questionsEnvelope = await client.GetQuestionsSuccesfully(u1PublicNotPublishedLink);
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(0, questionsEnvelope.Questions.First().AnswerOptions.Count);

            questionsEnvelope = await client.GetQuestionsSuccesfully(u1PublicPublishedLink);
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(0, questionsEnvelope.Questions.First().AnswerOptions.Count);

            questionsEnvelope = await client.GetQuestionsSuccesfully(u1PrivateNotPublishedLink);
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(0, questionsEnvelope.Questions.First().AnswerOptions.Count);

            questionsEnvelope = await client.GetQuestionsSuccesfully(u1PrivatePublishedLink);
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(0, questionsEnvelope.Questions.First().AnswerOptions.Count);

            await client.GetNotFound(u2PublicNotPublishedLink);

            questionsEnvelope = await client.GetQuestionsSuccesfully(u2PublicPublishedLink);
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(0, questionsEnvelope.Questions.First().AnswerOptions.Count);

            await client.GetNotFound(u2PrivateNotPublishedLink);
            await client.GetNotFound(u2PrivatePublishedLink);

            //includeAnswerOptions
            questionsEnvelope = await client.GetQuestionsSuccesfully($"{u1PublicNotPublishedLink}&includeAnswerOptions=true");
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(data.u1PublicNotPublished.Item5.AnswerOptions.Count, questionsEnvelope.Questions.First().AnswerOptions.Count);

            //  limit , offset
            for (int i = 0; i < 11; i++)
            {
                await helper.CreateQuestionInExam(
                    data.u1PublicNotPublished.Item1.Token,
                    data.u1PublicNotPublished.Item3.Id, i % 2 == 0);
            }

            //login user1
            client.Authorize(data.u1PublicNotPublished.Item1.Token);

            var limitOffsetTester = new LimitOffsetTester(client, u1PublicNotPublishedLink);
            await limitOffsetTester.DoTest(data.GetQuestions);

        }
    }
}