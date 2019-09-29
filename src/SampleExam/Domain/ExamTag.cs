using System;

namespace SampleExam.Domain
{
    public class ExamTag
    {
        public int ExamId { get; set; }
        public int TagId { get; set; }
        public Exam Exam { get; set; }
        public Tag Tag { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}