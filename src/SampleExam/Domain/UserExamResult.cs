using System;

namespace SampleExam.Domain
{
    public class UserExamResult
    {
        public int UserExamId { get; set; }
        public int QuestionCount { get; set; }
        public int RightAnswerCount { get; set; }
        public int WrongAnswerCount { get; set; }
        public bool IsPassed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserExam UserExam { get; set; }
    }
}