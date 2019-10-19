namespace SampleExam.Features.UserExam
{
    public class UserExamDTOEnvelope
    {
        public UserExamDTOEnvelope(UserExamDTO userExam) => this.UserExam = userExam;

        public UserExamDTO UserExam { get; }
    }
}