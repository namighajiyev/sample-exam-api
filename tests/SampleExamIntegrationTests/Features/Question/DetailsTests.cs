using SampleExam;
using SampleExamIntegrationTests.Helpers;
using SampleExamIntegrationTests.Helpers.Data;
using Xunit;

namespace SampleExamIntegrationTests.Features.Question
{
    public class DetailsTests : IntegrationTestBase
    {
        public DetailsTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void AllDetailsTests()
        {
            var client = httpClientFactory.CreateClient();
            var data = new QuestionTestData(client);

            var linkNoneExisting = $"/questions/{int.MaxValue}";
            var u1PublicNotPublishedLink = $"/questions/{data.u1PublicNotPublished.Item5.Id}";
            var u1PublicPublishedLink = $"/questions/{data.u1PublicPublished.Item5.Id}";
            var u1PrivateNotPublishedLink = $"/questions/{data.u1PrivateNotPublished.Item5.Id}";
            var u1PrivatePublishedLink = $"/questions/{data.u1PrivatePublished.Item5.Id}";

            var u2PublicNotPublishedLink = $"/questions/{data.u2PublicNotPublished.Item5.Id}";
            var u2PublicPublishedLink = $"/questions/{data.u2PublicPublished.Item5.Id}";
            var u2PrivateNotPublishedLink = $"/questions/{data.u2PrivateNotPublished.Item5.Id}";
            var u2PrivatePublishedLink = $"/questions/{data.u2PrivatePublished.Item5.Id}";
            client.Unauthorize();
            //non-existing
            await client.GetNotFound(linkNoneExisting);

            //user 1
            await client.GetNotFound(u1PublicNotPublishedLink);

            var questionDto = await client.GetQuestionSuccesfully(u1PublicPublishedLink);
            Assert.Equal(questionDto.Id, data.u1PublicPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            await client.GetNotFound(u1PrivateNotPublishedLink);
            await client.GetNotFound(u1PrivatePublishedLink);

            //user 2
            await client.GetNotFound(u2PublicNotPublishedLink);

            questionDto = await client.GetQuestionSuccesfully(u2PublicPublishedLink);
            Assert.Equal(questionDto.Id, data.u2PublicPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            await client.GetNotFound(u2PrivateNotPublishedLink);
            await client.GetNotFound(u2PrivatePublishedLink);

            questionDto = await client.GetQuestionSuccesfully($"{u1PublicPublishedLink}?includeAnswerOptions=true");
            Assert.Equal(questionDto.Id, data.u1PublicPublished.Item5.Id);
            Assert.Equal(data.u1PublicPublished.Item5.AnswerOptions.Count, questionDto.AnswerOptions.Count);

        }
    }
}