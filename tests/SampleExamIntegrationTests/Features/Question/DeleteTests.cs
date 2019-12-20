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
            var questionItems = await httpCallHelper.CreateQuestion();
            var questionItemsPublished = await httpCallHelper.CreateQuestion(loggedUser: questionItems.Item1);
            var questionItems2 = await httpCallHelper.CreateQuestion();
            var question1 = questionItems.Item5;
            var question2 = questionItems2.Item5;
            var questionPublished = questionItemsPublished.Item5;
            var examPublished = questionItemsPublished.Item3;
            var user1 = questionItems.Item1;

            var link1 = $"/questions/{question1.Id}";
            var link2 = $"/questions/{question2.Id}";
            var linkPublished = $"/questions/{questionPublished.Id}";
            var linkNoneExisting = $"/questions/{int.MaxValue}";

            await client.DeleteUnauthorized(link1);

            client.Authorize(user1.Token);

            await client.DeleteNotFound(linkNoneExisting);
            await client.DeleteNotFound(link2);
            await client.DeleteNotFound(linkNoneExisting);

            await client.DeleteSucessfully(link1);

            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                var count = dbContext.Questions.Where(e => e.Id == question1.Id).Count();
                Assert.Equal(count, 0);
                count = dbContext.Questions.Where(e => e.Id == question2.Id).Count();
                Assert.Equal(count, 1);
                count = dbContext.Questions.Where(e => e.Id == questionPublished.Id).Count();
                Assert.Equal(count, 1);

                foreach (var answerOption in question1.AnswerOptions)
                {
                    count = dbContext.AnswerOptions.Where(e => e.Id == answerOption.Id).Count();
                    Assert.Equal(count, 0);
                }

                foreach (var answerOption in question2.AnswerOptions)
                {
                    count = dbContext.AnswerOptions.Where(e => e.Id == answerOption.Id).Count();
                    Assert.Equal(count, 1);
                }

                foreach (var answerOption in questionPublished.AnswerOptions)
                {
                    count = dbContext.AnswerOptions.Where(e => e.Id == answerOption.Id).Count();
                    Assert.Equal(count, 1);
                }
            }


        }



        //delete published exam
        //delete normal case check answer options deleted too
    }
}