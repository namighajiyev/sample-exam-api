using System;

namespace SampleExam.Features.Answer
{
    public class AnswerOptionDTO
    {
        public int QuestionId { get; set; }

        public char Key { get; set; }

        public string Text { get; set; }

        public bool IsRight { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


    }
}