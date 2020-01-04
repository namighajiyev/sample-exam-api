using System;

namespace SampleExam.Features.UserExamResult
{
    public class UserExamResultDTO
    {
        public int UserExamId { get; set; }
        public int QuestionCount { get; set; }
        public int RightAnswerCount { get; set; }
        public int WrongAnswerCount { get; set; }
        public bool IsPassed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}