using System.Collections.Generic;

namespace SampleExam.Features.UserExam
{
    public class UserExamsDTOEnvelope
    {
        public UserExamsDTOEnvelope(IEnumerable<UserExamDTO> userExams, int userExamCount)
        {
            this.UserExams = userExams;
            this.UserExamCount = userExamCount;
        }

        public IEnumerable<UserExamDTO> UserExams { get; }
        public int UserExamCount { get; }
    }
}