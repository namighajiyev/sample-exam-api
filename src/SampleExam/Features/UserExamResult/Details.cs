using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;

namespace SampleExam.Features.UserExamResult
{
    public class Details
    {
        public class Query : IRequest<UserExamResultDTOEnvelope>
        {
            public int UserExamId { get; }

            public Query(int userExamId)
            {
                this.UserExamId = userExamId;
            }
        }


        public class QueryHandler : IRequestHandler<Query, UserExamResultDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;
            private readonly ICurrentUserAccessor currentUserAccessor;

            public QueryHandler(IMapper mapper, SampleExamContext context, ICurrentUserAccessor currentUserAccessor)
            {
                this.mapper = mapper;
                this.context = context;
                this.currentUserAccessor = currentUserAccessor;
            }
            public async Task<UserExamResultDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = currentUserAccessor.GetCurrentUserId();

                var userExam = await this.context.UserExams
                .Include(e => e.Exam).Where(e => e.Id == request.UserExamId && e.UserId == userId && e.Exam.IsPublished == true).FirstOrDefaultAsync();

                if (userExam == null)
                {
                    throw new Exceptions.UserExamNotFoundException();
                }

                var userExamResult = await context.UserExamResults.FirstOrDefaultAsync(e => e.UserExamId == request.UserExamId);
                if (userExamResult == null)
                {
                    throw new Exceptions.UserExamResultNotFoundException();
                }
                var userExamResultDto = mapper.Map<Domain.UserExamResult, UserExamResultDTO>(userExamResult);
                return new UserExamResultDTOEnvelope(userExamResultDto);
            }

        }


    }
}