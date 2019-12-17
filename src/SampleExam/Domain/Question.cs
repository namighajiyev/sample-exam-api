using System;
using System.Collections.Generic;

namespace SampleExam.Domain
{
    public class Question
    {
        public int Id { get; set; }

        public int ExamId { get; set; }

        public int QuestionTypeId { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Exam Exam { get; set; }
        public QuestionType QuestionType { get; set; }
        public ICollection<AnswerOption> AnswerOptions { get; set; }

    }
}