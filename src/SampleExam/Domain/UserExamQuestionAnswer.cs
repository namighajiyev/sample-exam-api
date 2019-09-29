using System;

namespace SampleExam.Domain
{
    public class UserExamQuestionAnswer
    {
        public int Id { get; set; }
        public int UserExamId { get; set; }
        public int AnswerOptionId { get; set; }
        public AnswerOption AnswerOption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}