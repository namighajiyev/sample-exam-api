using System;
using System.Collections.Generic;

namespace SampleExam.Domain
{
    public class Tag
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ExamTag> ExamTags { get; set; }

    }
}