using SampleExam;
using Xunit;

namespace SampleExamIntegrationTests.Features.QuestionAnswer
{
    public class CreateOrUpdateTests : IntegrationTestBase
    {
        public CreateOrUpdateTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        // [Fact]
        // public async void ShouldCreateTests()
        // {

        // }

        //unauthorized
        //answeroption not found
        //userexam not found
        //answeroption exam doesn't match userexam exam
        //case ended exam EndedAt.HasValue -should return error
        //case EndedAt.HasValue is false but examtimesInMinutes expired- should return error and set EndedAt
        //do for new answeroption- should add one
        //do for exactly same answeroption -should keep existing
        //do for question with different asweroption -should remove old and add new

        // =======================
        //UserExamNotFoundException 1 non existing
        //UserExamNotFoundException different user
        //UserExamAlreadyEndedException 1 really ended
        //UserExamAlreadyEndedException 2 time is too late
        //QuestionNotFoundException 1 none existing question
        //QuestionNotFoundException question of another exam
        //AnswerToRadioQuestionFormatException send multiple answerss to radio
        //AnswerOptionNotFoundException 
        //InvalidAnswerOptionExamException other questions answer option.
        //create, add , delete, add delete.




    }
}