using System;

namespace SampleExam.Features.Values
{
    public class ValueDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}