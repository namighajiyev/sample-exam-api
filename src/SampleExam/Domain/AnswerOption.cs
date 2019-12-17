using System;
using System.Collections.Generic;

namespace SampleExam.Domain
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }

        public string Text { get; set; }

        public bool IsRight { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


        public Question Question { get; set; }

        public ICollection<UserExamQuestion> UserExamQuestionAnswers { get; set; }

    }
}