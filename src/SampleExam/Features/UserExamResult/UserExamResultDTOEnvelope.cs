namespace SampleExam.Features.UserExamResult
{
    public class UserExamResultDTOEnvelope
    {
        public UserExamResultDTOEnvelope(UserExamResultDTO userExamResult) => this.UserExamResult = userExamResult;

        public UserExamResultDTO UserExamResult { get; }
    }
}