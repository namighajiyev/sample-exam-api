using System;

namespace SampleExam.Features.QuestionAnswer
{
    public class QuestionAnswerDTO
    {
        public int Id { get; set; }
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }

        public char Key { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}