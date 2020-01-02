using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Common;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Security;

namespace SampleExam.Features.UserExam
{
    public class List
    {

        public class Query : IRequest<UserExamsDTOEnvelope>
        {
            public Query(
                    int? limit,
                    int? offset,
                    bool? includeExams
                 )
            {
                this.Limit = limit ?? Constants.FETCH_LIMIT;
                this.Offset = offset ?? Constants.FETCH_OFFSET;
                this.IncludeExams = includeExams ?? false;

            }
            public int Limit { get; }
            public int Offset { get; }
            public bool IncludeExams { get; }
        }

        public class QueryHandler : IRequestHandler<Query, UserExamsDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;
            private readonly ICurrentUserAccessor currentUserAccessor;

            public QueryHandler(IMapper mapper, SampleExamContext context,
             ICurrentUserAccessor currentUserAccessor)
            {
                this.mapper = mapper;
                this.context = context;
                this.currentUserAccessor = currentUserAccessor;
            }
            public async Task<UserExamsDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {

                var userId = currentUserAccessor.GetCurrentUserId();
                var queryable = this.context.UserExams.AsNoTracking().Where(e => e.UserId == userId);
                if (request.IncludeExams)
                {
                    queryable = queryable.Include(e => e.Exam);
                }

                var userExams = await queryable
                .OrderByDescending(e => e.StartedtedAt)
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

                var userExamsCount = await queryable.CountAsync();

                var userExamDTOs = mapper.Map<List<Domain.UserExam>, List<UserExamDTO>>(userExams);

                return new UserExamsDTOEnvelope(userExamDTOs, userExamsCount);
            }
        }
    }
}
