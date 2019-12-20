using SampleExam;
using SampleExamIntegrationTests.Helpers;
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
            var httpCallHelper = new HttpCallHelper(client);
            var questionItems = await httpCallHelper.CreateQuestion();
            var questionItems2 = await httpCallHelper.CreateQuestion();
            var questionItemsPublished = await httpCallHelper.CreateQuestion(loggedUser: questionItems.Item1);
            var questionPrivateItems = await httpCallHelper.CreateQuestion(loggedUser: questionItems.Item1, isPrivate: true);
            var questionPrivateItemsPublished = await httpCallHelper.CreateQuestion(loggedUser: questionItems.Item1, isPrivate: true);

            var examPublished = questionItemsPublished.Item3;
            var examPrivatePublished = questionPrivateItemsPublished.Item3;


            var question1 = questionItems.Item5;
            var question2 = questionItems2.Item5;
            var questionPublished = questionItemsPublished.Item5;
            var questionPrivate = questionPrivateItems.Item5;
            var questionPrivatePublished = questionPrivateItems.Item5;

            var user1 = questionItems.Item1;

            var linkPrivateNonPublished1 = $"/questions/{question1.Id}";
            var linkPrivateNonPublished2 = $"/questions/{question2.Id}";
            var linkPublicPublished = $"/questions/{questionPublished.Id}";
            var linkPrivateNonPublished = $"/questions/{questionPrivate.Id}";
            var linkPrivatePublished = $"/questions/{questionPrivatePublished.Id}";
            var linkNoneExisting = $"/questions/{int.MaxValue}";

            client.Authorize(user1.Token);
            //publishing exams
            await httpCallHelper.PublishExam(examPublished.Id);
            await httpCallHelper.PublishExam(examPrivatePublished.Id);
            client.Unauthorize();

            await client.GetNotFound(linkPrivateNonPublished1);
            await client.GetNotFound(linkPrivateNonPublished2);
            await client.GetNotFound(linkPrivateNonPublished);
            await client.GetNotFound(linkPrivatePublished);
            await client.GetNotFound(linkNoneExisting);

            var questionDto = await client.GetQuestionSuccesfully(linkPublicPublished);
            Assert.Equal(questionDto.Id, questionPublished.Id);
            Assert.Equal(questionDto.AnswerOptions.Count, 0);

            questionDto = await client.GetQuestionSuccesfully($"{linkPublicPublished}?includeAnswerOptions=true");
            Assert.Equal(questionDto.Id, questionPublished.Id);
            Assert.Equal(questionDto.AnswerOptions.Count, questionPublished.AnswerOptions.Count);

        }
    }
}