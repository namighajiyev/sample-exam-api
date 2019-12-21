using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam;
using SampleExam.Features.Question;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Question
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
        public async void AllListTests()
        {
            var client = httpClientFactory.CreateClient();
            var data = new QuestionTestData(client);
            var helper = new HttpCallHelper(client);
            client.Unauthorize();
            var u1PublicNotPublishedLink = $"/questions?examId={data.u1PublicNotPublished.Item3.Id}";
            var u1PublicPublishedLink = $"/questions?examId={data.u1PublicPublished.Item3.Id}";
            var u1PrivateNotPublishedLink = $"/questions?examId={data.u1PrivateNotPublished.Item3.Id}";
            var u1PrivatePublishedLink = $"/questions?examId={data.u1PrivatePublished.Item3.Id}";

            var u2PublicNotPublishedLink = $"/questions?examId={data.u2PublicNotPublished.Item3.Id}";
            var u2PublicPublishedLink = $"/questions?examId={data.u2PublicPublished.Item3.Id}";
            var u2PrivateNotPublishedLink = $"/questions?examId={data.u2PrivateNotPublished.Item3.Id}";
            var u2PrivatePublishedLink = $"/questions?examId={data.u2PrivatePublished.Item3.Id}";

            var linkNoneExisting = $"/questions?examId={int.MaxValue}";
            await client.GetNotFound(linkNoneExisting);

            //user 1
            await client.GetNotFound(u1PublicNotPublishedLink);

            var questionsEnvelope = await client.GetQuestionsSuccesfully(u1PublicPublishedLink);
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(0, questionsEnvelope.Questions.First().AnswerOptions.Count);

            await client.GetNotFound(u1PrivateNotPublishedLink);
            await client.GetNotFound(u1PrivatePublishedLink);

            //user 2
            await client.GetNotFound(u2PublicNotPublishedLink);

            questionsEnvelope = await client.GetQuestionsSuccesfully(u2PublicPublishedLink);
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(0, questionsEnvelope.Questions.First().AnswerOptions.Count);

            await client.GetNotFound(u2PrivateNotPublishedLink);
            await client.GetNotFound(u2PrivatePublishedLink);

            //includeAnswerOptions
            questionsEnvelope = await client.GetQuestionsSuccesfully($"{u1PublicPublishedLink}&includeAnswerOptions=true");
            Assert.Equal(questionsEnvelope.QuestionCount, questionsEnvelope.Questions.Count());
            Assert.Equal(1, questionsEnvelope.QuestionCount);
            Assert.Equal(data.u1PublicPublished.Item5.AnswerOptions.Count, questionsEnvelope.Questions.First().AnswerOptions.Count);

            //  limit , offset
            for (int i = 0; i < 11; i++)
            {
                await helper.CreateQuestionInExam(
                    data.u1PublicNotPublished.Item1.Token,
                    data.u1PublicNotPublished.Item3.Id, i % 2 == 0);
            }
            client.Authorize(data.u1PublicNotPublished.Item1.Token);
            await helper.PublishExam(data.u1PublicNotPublished.Item3.Id);
            client.Unauthorize();
            var limitOffsetTester = new LimitOffsetTester(client, u1PublicPublishedLink);
            await limitOffsetTester.DoTest(data.GetQuestions);
        }


    }
}