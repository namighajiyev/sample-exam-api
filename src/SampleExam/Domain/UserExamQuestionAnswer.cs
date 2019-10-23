using System;

namespace SampleExam.Domain
{
    public class UserExamQuestionAnswer
    {
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerOptionId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserExam UserExam { get; set; }

        public AnswerOption AnswerOption { get; set; }

    }
}