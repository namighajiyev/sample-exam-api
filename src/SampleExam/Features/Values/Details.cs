using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Domain;
using SampleExam.Infrastructure;

namespace SampleExam.Features.Values
{
    public class Details
    {
        public class Query : IRequest<ValueDTOEnvelope>
        {
            public Query(int id)
            {
                this.Id = id;
            }

            public int Id { get; set; }
        }


        public class QueryHandler : IRequestHandler<Query, ValueDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;

            public QueryHandler(IMapper mapper, SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<ValueDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var value = await context.Values.Where(e => e.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                if (value == null)
                {
                    throw Exceptions.ValueNotFoundException;
                }

                var valueDto = mapper.Map<Value, ValueDTO>(value);

                return new ValueDTOEnvelope(valueDto);
            }
        }

    }


}