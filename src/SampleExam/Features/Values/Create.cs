using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
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

        public class Request : IRequest<ValueDTOEnvelope>
        {
            public ValueData Value { get; set; }
        }
        public class ValueDataValidator : AbstractValidator<ValueData>
        {
            public ValueDataValidator()
            {
                RuleFor(x => x.Text).NotNull().NotEmpty();
            }
        }
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Value).NotNull().SetValidator(new ValueDataValidator());
            }
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