namespace SampleExam.Infrastructure.Errors
{
    using Newtonsoft.Json;

    public class Error
    {
        public Error(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }


    }
}