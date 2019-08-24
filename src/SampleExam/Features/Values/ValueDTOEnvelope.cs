namespace SampleExam.Features.Values
{
    public class ValueDTOEnvelope
    {
        public ValueDTOEnvelope(ValueDTO value)
        {
            Value = value;
        }

        public ValueDTO Value { get; }
    }
}