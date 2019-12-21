using SampleExam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Question
{
    public class DetailsAuthTests : IntegrationTestBase
    {
        public DetailsAuthTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void AllDetailsAuthTests()
        {
            var client = httpClientFactory.CreateClient();
            var data = new QuestionTestData(client);

            var linkNoneExisting = $"/questions/private/{int.MaxValue}";
            var u1PublicNotPublishedLink = $"/questions/private/{data.u1PublicNotPublished.Item5.Id}";
            var u1PublicPublishedLink = $"/questions/private/{data.u1PublicPublished.Item5.Id}";
            var u1PrivateNotPublishedLink = $"/questions/private/{data.u1PrivateNotPublished.Item5.Id}";
            var u1PrivatePublishedLink = $"/questions/private/{data.u1PrivatePublished.Item5.Id}";

            var u2PublicNotPublishedLink = $"/questions/private/{data.u2PublicNotPublished.Item5.Id}";
            var u2PublicPublishedLink = $"/questions/private/{data.u2PublicPublished.Item5.Id}";
            var u2PrivateNotPublishedLink = $"/questions/private/{data.u2PrivateNotPublished.Item5.Id}";
            var u2PrivatePublishedLink = $"/questions/private/{data.u2PrivatePublished.Item5.Id}";



            client.Authorize(data.u1PublicNotPublished.Item1.Token);
            //non-existing
            await client.GetNotFound(linkNoneExisting);



            // logged user's
            var questionDto = await client.GetQuestionSuccesfully(u1PublicNotPublishedLink);
            Assert.Equal(questionDto.Id, data.u1PublicNotPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            questionDto = await client.GetQuestionSuccesfully(u1PublicPublishedLink);
            Assert.Equal(questionDto.Id, data.u1PublicPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            questionDto = await client.GetQuestionSuccesfully(u1PrivateNotPublishedLink);
            Assert.Equal(questionDto.Id, data.u1PrivateNotPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            questionDto = await client.GetQuestionSuccesfully(u1PrivatePublishedLink);
            Assert.Equal(questionDto.Id, data.u1PrivatePublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            // other user's
            await client.GetNotFound(u2PublicNotPublishedLink);

            questionDto = await client.GetQuestionSuccesfully(u2PublicPublishedLink);
            Assert.Equal(questionDto.Id, data.u2PublicPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            await client.GetNotFound(u2PrivateNotPublishedLink);
            await client.GetNotFound(u2PrivatePublishedLink);


            //includeAnswerOptions
            questionDto = await client.GetQuestionSuccesfully($"{u1PublicNotPublishedLink}?includeAnswerOptions=true");
            Assert.Equal(questionDto.Id, data.u1PublicNotPublished.Item5.Id);
            Assert.Equal(data.u1PublicNotPublished.Item5.AnswerOptions.Count, questionDto.AnswerOptions.Count);

        }
    }
}