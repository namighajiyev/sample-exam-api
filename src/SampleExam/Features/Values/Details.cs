using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SampleExam.Domain;

namespace SampleExam.Features.Values
{
    public class Details
    {
        public class Query : IRequest<ValueEnvelop>
        {
            public Query(int id)
            {
                this.Id = id;
            }

            public int Id { get; set; }
        }


        public class QueryHandler : IRequestHandler<Query, ValueEnvelop>
        {
            public Task<ValueEnvelop> Handle(Query request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new ValueEnvelop(new Value() { Id = 1, Text = "value1" }));
            }
        }

    }


}