using System;

namespace SampleExam.Domain
{
    public class Question
    {
        public int Id { get; set; }

        public int ExamId { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Exam Exam { get; set; }

    }
}