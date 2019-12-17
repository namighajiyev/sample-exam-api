using System;
using System.Collections.Generic;

namespace SampleExam.Features.Question
{
    public class QuestionDTO
    {

        public int Id { get; set; }

        public int ExamId { get; set; }
        public int QuestionTypeId { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<Answer.AnswerOptionDTO> AnswerOptions { get; set; }


    }
}