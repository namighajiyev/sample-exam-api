using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleExam;
using SampleExam.Infrastructure.Data;
using SampleExamIntegrationTests.Helpers;
using SampleExamIntegrationTests.Helpers.Data;
using Xunit;
using static SampleExam.Features.QuestionAnswer.CreateOrUpdate;

namespace SampleExamIntegrationTests.Features.QuestionAnswer
{
    public class CreateOrUpdateTests : IntegrationTestBase
    {
        private string link = "/questionanswers";
        public CreateOrUpdateTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void QuestionAnswerCreateOrUpdateTests()
        {
            var client = httpClientFactory.CreateClient();

            var data = new QuestionAnswerTestData(client);
            var httpCallHelper = new HttpCallHelper(client);
            var dbContextHelper = new DbContextHelper(this.dbContextFactory);

            client.Unauthorize();

            var request = data.User1.CreateRadioQuestion1AnswerRequest();
            await client.PostUnauthorized(link, request);

            client.Authorize(data.User1.User.Token);

            //UserExamNotFoundException 1 non existing
            request = data.NonExistingRequest;
            var problem = await client.PostNotFound(link, request);

            //UserExamNotFoundException different user
            request = data.User2.CreateRadioQuestion1AnswerRequest();
            problem = await client.PostNotFound(link, request);

            //UserExamAlreadyEndedException 1 really ended
            client.Authorize(data.User3.User.Token);
            await httpCallHelper.EndUserExam(data.User3.UserExam.Id);
            request = data.User3.CreateRadioQuestion1AnswerRequest();
            problem = await client.PostBadRequest(link, request);

            //UserExamAlreadyEndedException 2 time is too late
            client.Authorize(data.User4.User.Token);
            await dbContextHelper.UpdateUserExamStartDate(data.User4.UserExam.Id, data.User4.Exam.TimeInMinutes * -1);
            request = data.User4.CreateRadioQuestion1AnswerRequest();
            problem = await client.PostBadRequest(link, request);
            var userExam = await dbContextHelper.SelectUserExamAsync(data.User4.UserExam.Id);
            Assert.NotNull(userExam.EndedAt);

            //QuestionNotFoundException 1 none existing question
            client.Authorize(data.User1.User.Token);
            request = data.User1.CreateRadioQuestion1AnswerRequest();
            request.UserExamQuestionAnswer.QuestionId = int.MaxValue;
            problem = await client.PostNotFound(link, request);

            //QuestionNotFoundException question of another exam
            request = data.User1.CreateRadioQuestion1AnswerRequest();
            var userExamDTO = (await httpCallHelper.CreateUserExam(loggedUser: data.User1.User)).Item3;
            client.Authorize(data.User1.User.Token);
            request.UserExamQuestionAnswer.UserExamId = userExamDTO.Id;
            problem = await client.PostNotFound(link, request);

            //AnswerToRadioQuestionFormatException send multiple answerss to radio
            request = data.User1.CreateRadioQuestion1AnswerRequest();
            var answerOptionIds = new List<int>(request.UserExamQuestionAnswer.AnswerOptionIds);
            answerOptionIds.Add(data.User1.RadioQuestion1AnswerIds[0]);
            answerOptionIds.Add(data.User1.RadioQuestion1AnswerIds[1]);
            request.UserExamQuestionAnswer.AnswerOptionIds = answerOptionIds.Distinct().ToArray();
            problem = await client.PostBadRequest(link, request);

            //AnswerOptionNotFoundException 
            request = data.User1.CreateRadioQuestion1AnswerRequest(answerOptionIds: new int[] { int.MaxValue });
            problem = await client.PostNotFound(link, request);

            //InvalidAnswerOptionExamException other questions answer option.
            request = data.User1.CreateRadioQuestion1AnswerRequest(answerOptionIds: new int[] { data.User2.RadioQuestion2AnswerIds.First() });
            problem = await client.PostBadRequest(link, request);

            await UserUpdateQuestionAnswersTest(data.User1, client);

            client.Authorize(data.User2.User.Token);
            await UserUpdateQuestionAnswersTest(data.User2, client);


        }

        private async Task UserUpdateQuestionAnswersTest(UserQuestionAnswerData user, System.Net.Http.HttpClient client)
        {
            //radio
            var request = user.CreateRadioQuestion1AnswerRequest();
            var questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            AssertHelper.AssertEqual(questionAnswer, request.UserExamQuestionAnswer);
            var questionAnswerOld = questionAnswer;

            questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            AssertHelper.AssertEqual(questionAnswer, request.UserExamQuestionAnswer);
            AssertHelper.AssertEqual(questionAnswer, questionAnswerOld);
            questionAnswerOld = questionAnswer;

            request = user.CreateRadioQuestion1AnswerRequest(questionAnswerOld.AnswerOptions.First().AnswerOptionId);
            questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            AssertHelper.AssertEqual(questionAnswer, request.UserExamQuestionAnswer);
            AssertHelper.AssertNotEqual(questionAnswer, questionAnswerOld);
            questionAnswerOld = questionAnswer;

            request = user.CreateRadioQuestion1AnswerRequest(questionAnswerOld.AnswerOptions.First().AnswerOptionId);
            questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            AssertHelper.AssertEqual(questionAnswer, request.UserExamQuestionAnswer);
            AssertHelper.AssertNotEqual(questionAnswer, questionAnswerOld);
            questionAnswerOld = questionAnswer;

            //checkbox
            request = user.CreateCheckboxQuestion1AnswerRequest();
            questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            AssertHelper.AssertEqual(questionAnswer, request.UserExamQuestionAnswer);
            questionAnswerOld = questionAnswer;

            questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            AssertHelper.AssertEqual(questionAnswer, request.UserExamQuestionAnswer);
            AssertHelper.AssertEqual(questionAnswer, questionAnswerOld);
            questionAnswerOld = questionAnswer;

            request = user.CreateCheckboxQuestion1AnswerRequest(questionAnswerOld.AnswerOptions.First().AnswerOptionId);
            questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            AssertHelper.AssertEqual(questionAnswer, request.UserExamQuestionAnswer);
            AssertHelper.AssertNotEqual(questionAnswer, questionAnswerOld);
            questionAnswerOld = questionAnswer;

            request = user.CreateCheckboxQuestion1AnswerRequest(questionAnswerOld.AnswerOptions.First().AnswerOptionId);
            questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            AssertHelper.AssertEqual(questionAnswer, request.UserExamQuestionAnswer);
            AssertHelper.AssertNotEqual(questionAnswer, questionAnswerOld);
            questionAnswerOld = questionAnswer;


        }

    }
}