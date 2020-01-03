using System.Linq;
using SampleExam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Question
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
        public async void ShouldDeleteTests()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContextHelper = new DbContextHelper(this.dbContextFactory);
            var questionItems = await httpCallHelper.CreateQuestion();
            var questionItemsPublished = await httpCallHelper.CreateQuestion(loggedUser: questionItems.Item1);
            var examPublished = questionItemsPublished.Item3;
            var questionItems2 = await httpCallHelper.CreateQuestion();
            var question1 = questionItems.Item5;
            var question2 = questionItems2.Item5;
            var questionPublished = questionItemsPublished.Item5;

            var user1 = questionItems.Item1;

            var link1 = $"/questions/{question1.Id}";
            var link2 = $"/questions/{question2.Id}";
            var linkPublished = $"/questions/{questionPublished.Id}";
            var linkNoneExisting = $"/questions/{int.MaxValue}";

            await client.DeleteUnauthorized(link1);

            client.Authorize(user1.Token);

            //publishing exam 
            await httpCallHelper.PublishExam(examPublished.Id);

            await client.DeleteNotFound(linkNoneExisting);
            await client.DeleteNotFound(link2);
            await client.DeleteNotFound(linkPublished);

            await client.DeleteSucessfully(link1);


            var count = dbContextHelper.SelectQuestionCount(question1.Id);
            Assert.Equal(0, count);
            count = dbContextHelper.SelectQuestionCount(question2.Id);
            Assert.Equal(1, count);
            count = dbContextHelper.SelectQuestionCount(questionPublished.Id);
            Assert.Equal(1, count);

            foreach (var answerOption in question1.AnswerOptions)
            {
                count = dbContextHelper.SelectAnswerOptionCount(answerOption.Id);
                Assert.Equal(0, count);
            }

            foreach (var answerOption in question2.AnswerOptions)
            {
                count = dbContextHelper.SelectAnswerOptionCount(answerOption.Id);
                Assert.Equal(1, count);
            }

            foreach (var answerOption in questionPublished.AnswerOptions)
            {
                count = dbContextHelper.SelectAnswerOptionCount(answerOption.Id);
                Assert.Equal(1, count);
            }


        }
    }
}