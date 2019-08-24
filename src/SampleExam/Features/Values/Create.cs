using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SampleExam.Domain;
using SampleExam.Infrastructure;

namespace SampleExam.Features.Values
{
    public class Create
    {
        public class ValueData
        {
            public string Text { get; set; }
        }

        public class Command : IRequest<ValueDTOEnvelope>
        {
            public ValueData Value { get; set; }
        }

        public class Handler : IRequestHandler<Command, ValueDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;

            public Handler(IMapper mapper, SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<ValueDTOEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var value = new Value()
                {
                    Text = request.Value.Text,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await this.context.Values.AddAsync(value, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                var valueDto = mapper.Map<Value, ValueDTO>(value);

                return new ValueDTOEnvelope(valueDto);
            }
        }
    }
}