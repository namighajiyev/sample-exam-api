using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure;
using SampleExam.Infrastructure.Errors;
using System.Linq;

namespace SampleExam.Features.Values
{
    public class Delete
    {
        public class Request : IRequest
        {
            public Request(int id)
            {
                this.Id = id;
            }

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private SampleExamContext context;

            public Handler(SampleExamContext context) => this.context = context;
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var value = await context.Values.Where(e => e.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                if (value == null)
                {
                    throw Exceptions.ValueNotFoundException;
                }

                context.Values.Remove(value);
                await context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}