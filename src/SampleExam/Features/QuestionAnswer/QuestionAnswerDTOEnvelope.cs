namespace SampleExam.Features.QuestionAnswer
{
    public class QuestionAnswerDTOEnvelope
    {
        public QuestionAnswerDTOEnvelope(QuestionAnswerDTO questionAnswer) => this.QuestionAnswer = questionAnswer;

        public QuestionAnswerDTO QuestionAnswer { get; }
    }
}