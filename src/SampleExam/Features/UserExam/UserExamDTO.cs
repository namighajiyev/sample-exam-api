using System;
using SampleExam.Features.Exam;

namespace SampleExam.Features.UserExam
{
    public class UserExamDTO
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public DateTime StartedtedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public ExamDTO Exam { get; set; }
    }
}