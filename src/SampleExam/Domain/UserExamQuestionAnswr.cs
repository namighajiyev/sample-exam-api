using System;

namespace SampleExam.Domain
{
    //todo after refactor rename this to UserExamQuestionAnswer
    public class UserExamQuestionAnswr
    {
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerOptionId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserExam UserExam { get; set; }
        public Question Question { get; set; }
        public AnswerOption AnswerOption { get; set; }

        public UserExamQuestionAnswr UserExamQuestionAnswer { get; set; }

    }
}