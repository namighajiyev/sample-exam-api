using System;
using System.Collections.Generic;

namespace SampleExam.Domain
{
    public class Tag
    {
        public string TagId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ExamTag> ExamTags { get; set; }

    }
}