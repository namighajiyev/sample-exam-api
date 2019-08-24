using System.Collections.Generic;
using SampleExam.Domain;

namespace SampleExam.Features.Values
{
    public class ValuesDTOEnvelop
    {
        public List<ValueDTO> Values { get; set; }

        public int ValuesCount { get; set; }
    }
}