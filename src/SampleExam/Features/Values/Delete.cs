using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure;
using SampleExam.Infrastructure.Errors;
using System.Linq;
using AutoMapper;
using SampleExam.Domain;

namespace SampleExam.Features.Values
{
    public class Delete
    {
        public class Request : IRequest<ValueDTOEnvelope>
        {
            public Request(int id)
            {
                this.Id = id;
            }

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Request, ValueDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;

            public Handler(IMapper mapper, SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<ValueDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var value = await context.Values.Where(e => e.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                if (value == null)
                {
                    throw new Exceptions.ValueNotFoundException();
                }

                context.Values.Remove(value);
                await context.SaveChangesAsync(cancellationToken);

                var valueDto = mapper.Map<Value, ValueDTO>(value);

                return new ValueDTOEnvelope(valueDto);
            }
        }
    }
}