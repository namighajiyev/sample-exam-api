using System.Collections.Generic;

namespace SampleExam.Features.Question
{
    public class QuestionsDTOEnvelope
    {
        public QuestionsDTOEnvelope(IEnumerable<QuestionDTO> questions, int questionCount)
        {
            this.Questions = questions;
            this.QuestionCount = questionCount;
        }

        public IEnumerable<QuestionDTO> Questions { get; }
        public int QuestionCount { get; }
    }
}