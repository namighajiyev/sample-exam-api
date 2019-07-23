using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SampleExam.Domain;

namespace SampleExam.Features.Values
{
    public class List
    {
        public class Query : IRequest<ValuesEnvelop>
        {
            public Query(int? limit, int? offset)
            {
                this.Limit = limit;
                this.Offset = offset;
            }

            public int? Limit { get; }
            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, ValuesEnvelop>
        {
            public Task<ValuesEnvelop> Handle(Query request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new ValuesEnvelop()
                {
                    Values = new List<Value>() {
                                new Value() {Id = 1, Text = "value1"},
                                new Value() { Id = 2, Text = "value2" },
                                },
                    ValuesCount = 2
                });
            }
        }
    }

}