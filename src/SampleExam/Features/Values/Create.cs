using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SampleExam.Domain;

namespace SampleExam.Features.Values
{
    public class Create
    {
        public class ValueData
        {
            public string Text { get; set; }
        }

        public class Command : IRequest<ValueEnvelop>
        {
            public ValueData Value { get; set; }
        }

        public class Handler : IRequestHandler<Command, ValueEnvelop>
        {
            public Task<ValueEnvelop> Handle(Command request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new ValueEnvelop(new Value() { Id = 3, Text = request.Value.Text }));
            }
        }
    }
}