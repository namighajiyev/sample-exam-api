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
            var httpCallHelper = new HttpCallHelper(client);
            //u means user
            var u1PublicNotPublished = await httpCallHelper.CreateQuestion();
            var u1PublicPublished = await httpCallHelper.CreateQuestion(loggedUser: u1PublicNotPublished.Item1);
            var u1PrivateNotPublished = await httpCallHelper.CreateQuestion(loggedUser: u1PublicNotPublished.Item1, isPrivate: true);
            var u1PrivatePublished = await httpCallHelper.CreateQuestion(loggedUser: u1PublicNotPublished.Item1, isPrivate: true);

            var u2PublicNotPublished = await httpCallHelper.CreateQuestion();
            var u2PublicPublished = await httpCallHelper.CreateQuestion(loggedUser: u2PublicNotPublished.Item1);
            var u2PrivateNotPublished = await httpCallHelper.CreateQuestion(loggedUser: u2PublicNotPublished.Item1, isPrivate: true);
            var u2PrivatePublished = await httpCallHelper.CreateQuestion(loggedUser: u2PublicNotPublished.Item1, isPrivate: true);

            var linkNoneExisting = $"/questions/private/{int.MaxValue}";

            var u1PublicNotPublishedLink = $"/questions/private/{u1PublicNotPublished.Item5.Id}";
            var u1PublicPublishedLink = $"/questions/private/{u1PublicPublished.Item5.Id}";
            var u1PrivateNotPublishedLink = $"/questions/private/{u1PrivateNotPublished.Item5.Id}";
            var u1PrivatePublishedLink = $"/questions/private/{u1PrivatePublished.Item5.Id}";

            var u2PublicNotPublishedLink = $"/questions/private/{u2PublicNotPublished.Item5.Id}";
            var u2PublicPublishedLink = $"/questions/private/{u2PublicPublished.Item5.Id}";
            var u2PrivateNotPublishedLink = $"/questions/private/{u2PrivateNotPublished.Item5.Id}";
            var u2PrivatePublishedLink = $"/questions/private/{u2PrivatePublished.Item5.Id}";

            //publishing exams
            client.Authorize(u1PublicNotPublished.Item1.Token);
            await httpCallHelper.PublishExam(u1PublicPublished.Item3.Id);
            await httpCallHelper.PublishExam(u1PrivatePublished.Item3.Id);

            client.Authorize(u2PublicNotPublished.Item1.Token);
            await httpCallHelper.PublishExam(u2PublicPublished.Item3.Id);
            await httpCallHelper.PublishExam(u2PrivatePublished.Item3.Id);

            client.Authorize(u1PublicNotPublished.Item1.Token);
            //non-existing
            await client.GetNotFound(linkNoneExisting);



            // logged user's
            var questionDto = await client.GetQuestionSuccesfully(u1PublicNotPublishedLink);
            Assert.Equal(questionDto.Id, u1PublicNotPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            questionDto = await client.GetQuestionSuccesfully(u1PublicPublishedLink);
            Assert.Equal(questionDto.Id, u1PublicPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            questionDto = await client.GetQuestionSuccesfully(u1PrivateNotPublishedLink);
            Assert.Equal(questionDto.Id, u1PrivateNotPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            questionDto = await client.GetQuestionSuccesfully(u1PrivatePublishedLink);
            Assert.Equal(questionDto.Id, u1PrivatePublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            // other user's
            await client.GetNotFound(u2PublicNotPublishedLink);

            questionDto = await client.GetQuestionSuccesfully(u2PublicPublishedLink);
            Assert.Equal(questionDto.Id, u2PublicPublished.Item5.Id);
            Assert.Equal(0, questionDto.AnswerOptions.Count);

            await client.GetNotFound(u2PrivateNotPublishedLink);
            await client.GetNotFound(u2PrivatePublishedLink);


            //includeAnswerOptions
            questionDto = await client.GetQuestionSuccesfully($"{u1PublicNotPublishedLink}?includeAnswerOptions=true");
            Assert.Equal(questionDto.Id, u1PublicNotPublished.Item5.Id);
            Assert.Equal(u1PublicNotPublished.Item5.AnswerOptions.Count, questionDto.AnswerOptions.Count);

        }
    }
}