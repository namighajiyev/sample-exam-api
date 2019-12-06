namespace SampleExamIntegrationTests.Features.Question
{
    public class EditTests
    {
        //unauthorized
        //bad request question text 1) empty, 2) null 3) length more than Constants.QUESTION_TEXT_LEN
        //bad request 1) answer empty, 2) answer null, 3) answers les than Constants.QUESTION_ANSWER_MIN_COUNT 
        //4) answers more than Constants.QUESTION_ANSWER_MAX_COUNT 5) with all answer wrong 
        //6) with all answers right
        //bad request 1) answer option text empty 2) ao text null 3) ao text > Constants.ANSWEROPTION_TEXT_LEN 
        //4) ao isright null
        //edit  other users exam
        //edit published exam
        //normal case
        // test with question text null - e.g remains the same
        // test answeroptions is null - e.g remains the same
    }
}