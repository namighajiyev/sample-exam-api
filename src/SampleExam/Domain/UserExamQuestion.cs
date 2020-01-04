using System;
using System.Collections.Generic;

namespace SampleExam.Domain
{
    public class UserExamQuestion
    {
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }
        public bool HasRightAnswer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserExam UserExam { get; set; }
        public Question Question { get; set; }
        public ICollection<UserExamQuestionAnswr> UserExamQuestionAnswers { get; set; }

    }
}