using System.Collections.Generic;

namespace SampleExam.Features.Exam
{
    public class ExamsDTOEnvelope
    {
        public ExamsDTOEnvelope(IEnumerable<ExamDTO> exams, int examCount)
        {
            this.Exams = exams;
            this.ExamCount = examCount;
        }

        public IEnumerable<ExamDTO> Exams { get; }
        public int ExamCount { get; }
    }
}