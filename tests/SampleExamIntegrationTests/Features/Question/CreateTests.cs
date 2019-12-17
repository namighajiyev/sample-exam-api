using Xunit;
using SampleExam;
using SampleExamIntegrationTests.Helpers;
using SampleExam.Features.Question;


namespace SampleExamIntegrationTests.Features.Question
{
    public class CreateTests : IntegrationTestBase
    {
        public CreateTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldCreateTests()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();
            var notPublishedExamItems = await httpCallHelper.CreateExam();
            var user1 = notPublishedExamItems.Item1;
            var examDto1 = notPublishedExamItems.Item3;
            var examDto2 = (await httpCallHelper.CreateExam()).Item3;
            var examDto3Published = (await httpCallHelper.CreatePublishedExam(loggedUser: user1)).Item2;
            var radioQuestionData = TestData.Question.Create.NewQuestionData();
            var checkboxQuestionData = TestData.Question.Create.NewQuestionData();
            var link1 = $"/questions/{examDto1.Id}";
            var link2 = $"/questions/{examDto2.Id}";
            var link3Published = $"/questions/{examDto3Published.Id}";
            await client.PostUnauthorized(link1, new Create.Request() { Question = radioQuestionData });

            client.Authorize(user1.Token);

            var badQuestion = TestData.Question.Create.NewQuestionDataWithEmptyText();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithNullText();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithTooLongText();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithEmptyAnswerOption();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithNullAnswerOption();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithFewerAnswerOption();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithMoreAnswerOption();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataRadioWithTwoRight();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataCheckboxWithSingleRight();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithAllRight();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithAllWrong();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithAllRight(false);
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithAllWrong(false);
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithEmptyAnswerText();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithNullAnswerText();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            badQuestion = TestData.Question.Create.NewQuestionDataWithTooLongAnswerText();
            await client.PostBadRequest(link1, new Create.Request() { Question = badQuestion });

            //other user's exam 
            await client.PostNotFound(link2, new Create.Request() { Question = radioQuestionData });

            //add to published exam
            await client.PostNotFound(link3Published, new Create.Request() { Question = radioQuestionData });

            //success
            await client.PostSucessfully(link1, new Create.Request() { Question = radioQuestionData });
            await client.PostSucessfully(link1, new Create.Request() { Question = checkboxQuestionData });

        }

        //add to other users exam
        //add to published exam
        //normal case

    }
}