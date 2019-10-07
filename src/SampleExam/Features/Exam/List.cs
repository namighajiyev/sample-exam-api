using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;
using SampleExam.Infrastructure;
using SampleExam.Common;



namespace SampleExam.Features.Exam
{
    public class List
    {

        public class Query : IRequest<ExamsDTOEnvelope>
        {
            public Query(int? userId,
            int? limit,
            int? offset,
            bool? isCurrentUser,
            bool? includeTags,
            bool? includeUser
            )
            {
                this.UserId = userId;
                this.Limit = limit ?? Constants.FETCH_LIMIT;
                this.Offset = offset ?? Constants.FETCH_OFFSET;
                this.IsCurrentUser = isCurrentUser ?? false;
                this.IncludeTags = includeTags ?? false;
                this.IncludeUser = includeUser ?? false;

            }

            public int? UserId { get; }
            public int Limit { get; private set; }
            public int Offset { get; private set; }
            public bool IsCurrentUser { get; private set; }
            public bool IncludeTags { get; private set; }
            public bool IncludeUser { get; private set; }
        }
        public class QueryHandler : IRequestHandler<Query, ExamsDTOEnvelope>
        {
            private IMapper mapper;
            private ICurrentUserAccessor currentUserAccessor;
            private SampleExamContext context;

            public QueryHandler(IMapper mapper, ICurrentUserAccessor currentUserAccessor,
             SampleExamContext context)
            {
                this.mapper = mapper;
                this.currentUserAccessor = currentUserAccessor;
                this.context = context;
            }
            public async Task<ExamsDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                int? userId = request.UserId ?? (request.IsCurrentUser ? (int?)currentUserAccessor.GetCurrentUserId() : null);
                var queryable = context.Exams.AsNoTracking();

                if (request.IncludeTags)
                {
                    queryable = queryable.Include(e => e.ExamTags).ThenInclude(e => e.Tag);
                }

                if (request.IncludeUser)
                {
                    queryable = queryable.Include(e => e.User);
                }


                if (!request.IsCurrentUser)
                {
                    queryable = queryable.Where(e => e.IsPublished);
                    queryable = queryable.Where(e => !e.IsPrivate);
                }

                if (userId.HasValue)
                {
                    queryable = queryable.Where(e => e.UserId == userId.Value);
                }

                var exams = await queryable
                .OrderByDescending(e => e.CreatedAt)
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

                var examCount = await queryable.CountAsync();

                var examDTOs = mapper.Map<List<Domain.Exam>, List<ExamDTO>>(exams);

                return new ExamsDTOEnvelope(examDTOs, examCount);
            }
        }

    }
}