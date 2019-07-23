using Newtonsoft.Json;

namespace SampleExam.Domain
{
    public class Value
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Text { get; set; }

    }

}