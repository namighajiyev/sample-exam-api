using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SampleExam.Infrastructure.Errors
{
    public class ApiProblemDetails : ProblemDetails
    {
        [JsonProperty(PropertyName = "errors")]
        public IDictionary<string, ICollection<Error>> Errors { get; } = new Dictionary<string, ICollection<Error>>();
    }
}