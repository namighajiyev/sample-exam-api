using Newtonsoft.Json;
using System;

namespace SampleExam.Domain
{
    public class Value
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }

}