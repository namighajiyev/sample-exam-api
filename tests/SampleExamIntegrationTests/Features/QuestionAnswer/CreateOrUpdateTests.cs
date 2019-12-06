namespace SampleExamIntegrationTests.Features.QuestionAnswer
{
    public class CreateOrUpdateTests
    {
        //unauthorized
        //answeroption not found
        //userexam not found
        //answeroption exam doesn't match userexam exam
        //case ended exam EndedAt.HasValue -should return error
        //case EndedAt.HasValue is false but examtimesInMinutes expired- should return error and set EndedAt
        //do for new answeroption- should add one
        //do for exactly same answeroption -should keep existing
        //do for question with different asweroption -should remove old and add new
    }
}