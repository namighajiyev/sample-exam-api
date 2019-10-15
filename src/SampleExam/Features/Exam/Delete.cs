using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure;
using SampleExam.Infrastructure.Errors;

namespace SampleExam.Features.Exam
{
    public class Delete
    {
        public class Request : IRequest<ExamDTOEnvelope>
        {
            public Request(int id)
            {
                this.Id = id;
            }

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Request, ExamDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;
            private readonly ICurrentUserAccessor currentUserAccessor;

            public Handler(IMapper mapper, SampleExamContext context, ICurrentUserAccessor currentUserAccessor)
            {
                this.mapper = mapper;
                this.context = context;
                this.currentUserAccessor = currentUserAccessor;
            }
            public async Task<ExamDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = currentUserAccessor.GetCurrentUserId();
                var exam = await context.Exams.ByIdAndUserId(request.Id, userId).FirstOrDefaultAsync(cancellationToken);
                if (exam == null)
                {
                    throw new Exceptions.ExamNotFoundException();
                }

                exam.IsDeleted = true;
                exam.DeletedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(cancellationToken);
                var examDto = mapper.Map<Domain.Exam, ExamDTO>(exam);
                return new ExamDTOEnvelope(examDto);
            }
        }
    }
}