using System;
using System.Collections.Generic;

namespace SampleExam.Features.QuestionAnswer
{
    public class QuestionAnswerOptionDTO
    {
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerOptionId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
    public class QuestionAnswerDTO
    {
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }
        public IEnumerable<QuestionAnswerOptionDTO> AnswerOptions { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}