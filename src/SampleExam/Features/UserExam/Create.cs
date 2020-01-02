using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Validation.Common;

namespace SampleExam.Features.UserExam
{
    public class Create
    {
        public class Request : IRequest<UserExamDTOEnvelope>
        {
            public Request(int examId)
            {
                this.ExamId = examId;
            }

            public int ExamId { get; }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.ExamId).Id<Request, int>(errorCodePrefix + "UserExamExam");
            }
        }

        public class Handler : IRequestHandler<Request, UserExamDTOEnvelope>
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

            public async Task<UserExamDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = this.currentUserAccessor.GetCurrentUserId();
                var exam = context.Exams.Where(e => e.Id == request.ExamId && e.IsPublished == true).FirstOrDefault();

                if (exam == null)
                {
                    throw new Exceptions.ExamNotFoundException();
                }
                if (exam.IsPrivate && exam.UserId != userId)
                {
                    throw new Exceptions.ExamNotFoundException();
                }

                var userExam = new Domain.UserExam();
                userExam.ExamId = request.ExamId;
                userExam.UserId = userId;
                userExam.StartedtedAt = DateTime.UtcNow;

                await this.context.UserExams.AddAsync(userExam, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                context.Entry(userExam).Reload();
                var userExamDto = mapper.Map<Domain.UserExam, UserExamDTO>(userExam);
                return new UserExamDTOEnvelope(userExamDto);
            }


        }
    }
}