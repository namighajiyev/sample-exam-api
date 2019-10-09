namespace SampleExam.Features.Exam
{
    public class ExamDTOEnvelope
    {
        public ExamDTOEnvelope(ExamDTO exam) => this.Exam = exam;

        public ExamDTO Exam { get; }
    }
}