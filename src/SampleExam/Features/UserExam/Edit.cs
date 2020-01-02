using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Validation.Common;

namespace SampleExam.Features.UserExam
{
    public class Edit
    {
        public class Request : IRequest<UserExamDTOEnvelope>
        {
            public Request(int id)
            {
                this.Id = id;
            }

            public int Id { get; }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.Id).Id<Request, int>(errorCodePrefix + "UserExamExam");
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

                var userExam = await this.context.UserExams
                .Include(e => e.Exam).Where(e => e.Id == request.Id && e.UserId == userId && e.Exam.IsPublished == true).FirstOrDefaultAsync();

                if (userExam == null)
                {
                    throw new Exceptions.UserExamNotFoundException();
                }

                if (userExam.Exam.IsPrivate && userExam.Exam.UserId != userId)
                {
                    throw new Exceptions.PrivateUserExamEditException();
                }

                if (userExam.EndedAt.HasValue)
                {
                    throw new Exceptions.UserExamAlreadyEndedException();
                }


                userExam.EndedAt = DateTime.UtcNow;
                await context.SaveChangesAsync(cancellationToken);
                context.Entry(userExam).Reload();
                var userExamDto = mapper.Map<Domain.UserExam, UserExamDTO>(userExam);
                return new UserExamDTOEnvelope(userExamDto);
            }

        }
    }
}