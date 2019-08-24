using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SampleExam.Domain;
using SampleExam.Infrastructure;

namespace SampleExam.Features.Values
{
    public class Edit
    {
        public class Request : IRequest<ValueDTOEnvelope>
        {
            [JsonIgnore]
            public int Id { get; set; }
            public string Text { get; set; }
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
                    throw Exceptions.ValueNotFoundException;
                }

                value.Text = request.Text;
                value.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(cancellationToken);

                var valueDto = mapper.Map<Value, ValueDTO>(value);

                return new ValueDTOEnvelope(valueDto);
            }
        }
    }
}