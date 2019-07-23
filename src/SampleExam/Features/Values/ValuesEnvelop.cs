using System.Collections.Generic;
using SampleExam.Domain;

namespace SampleExam.Features.Values
{
    public class ValuesEnvelop
    {
        public List<Value> Values { get; set; }

        public int ValuesCount { get; set; }
    }
}