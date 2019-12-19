using System;

namespace SampleExam.Features.Answer
{
    public class AnswerOptionDTO
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }

        public string Text { get; set; }

        public bool IsRight { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


    }
}