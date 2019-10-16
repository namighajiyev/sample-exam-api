using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SampleExam.Domain;
 
using SampleExam.Infrastructure.Data;

namespace SampleExam.Features.Values
{
    public class Edit
    {

        public class ValueData
        {
            [JsonIgnore]
            public int Id { get; set; }
            public string Text { get; set; }
        }

        public class Request : IRequest
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
        public class Handler : IRequestHandler<Request>
        {
            private IMapper mapper;
            private SampleExamContext context;

            public Handler(IMapper mapper, SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var value = await context.Values.Where(e => e.Id == request.Value.Id).FirstOrDefaultAsync(cancellationToken);
                if (value == null)
                {
                    throw new Exceptions.ValueNotFoundException();
                }

                value.Text = request.Value.Text;
                value.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}