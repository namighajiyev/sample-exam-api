using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Domain;
 
using SampleExam.Infrastructure.Data;

namespace SampleExam.Features.Values
{
    public class List
    {
        public class Query : IRequest<ValuesDTOEnvelop>
        {
            public Query(int? limit, int? offset)
            {
                this.Limit = limit;
                this.Offset = offset;
            }

            public int? Limit { get; }
            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, ValuesDTOEnvelop>
        {
            private IMapper mapper;
            private SampleExamContext context;

            public QueryHandler(IMapper mapper, SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<ValuesDTOEnvelop> Handle(Query request, CancellationToken cancellationToken)
            {
                var values = await context.Values.AsNoTracking().ToListAsync(cancellationToken);
                var valueList = mapper.Map<List<Value>, List<ValueDTO>>(values);

                return new ValuesDTOEnvelop() { Values = valueList, ValuesCount = valueList.Count };
            }
        }
    }

}