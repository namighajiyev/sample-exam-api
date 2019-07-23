using SampleExam.Domain;

namespace SampleExam.Features.Values
{
    public class ValueEnvelop
    {
        public ValueEnvelop(Value value)
        {
            Value = value;
        }

        public Value Value { get; }
    }
}
