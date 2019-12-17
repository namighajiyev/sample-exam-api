using System;
using System.Collections.Generic;

namespace SampleExam.Features.QuestionAnswer
{
    public class QuestionAnswerDTO
    {
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }
        public IEnumerable<int> AnswerOptionIds { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}