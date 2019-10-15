namespace SampleExam.Features.Question
{
    public class QuestionDTOEnvelope
    {
        public QuestionDTOEnvelope(QuestionDTO question) => this.Question = question;
        public QuestionDTO Question { get; }
    }
}