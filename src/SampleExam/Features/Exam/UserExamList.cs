using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using SampleExam.Common;
using static SampleExam.Features.Exam.Enums;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Data;

namespace SampleExam.Features.Exam
{
    public class UserExamList
    {

        public class Query : IRequest<ExamsDTOEnvelope>
        {
            public Query(
            PublishType publishType,
            PrivateType privateType,
            int? limit,
            int? offset,
            bool? includeTags,
            bool? includeUser
            )
            {
                this.PublishType = publishType;
                this.PrivateType = privateType;
                this.Limit = limit ?? Constants.FETCH_LIMIT;
                this.Offset = offset ?? Constants.FETCH_OFFSET;
                this.IncludeTags = includeTags ?? false;
                this.IncludeUser = includeUser ?? false;

            }
            public PublishType PublishType { get; }
            public PrivateType PrivateType { get; }
            public int Limit { get; }
            public int Offset { get; }
            public bool IncludeTags { get; }
            public bool IncludeUser { get; }
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
                int userId = currentUserAccessor.GetCurrentUserId();

                var queryable = context.Exams.AsNoTracking();

                queryable = queryable.Where(e => e.UserId == userId);

                switch (request.PublishType)
                {
                    case PublishType.Published:
                        queryable = queryable.Where(e => e.IsPublished);
                        break;
                    case PublishType.NotPublished:
                        queryable = queryable.Where(e => !e.IsPublished);
                        break;
                    case PublishType.All:
                    default:
                        break;
                }

                switch (request.PrivateType)
                {
                    case PrivateType.Private:
                        queryable = queryable.Where(e => e.IsPrivate);
                        break;
                    case PrivateType.Public:
                        queryable = queryable.Where(e => !e.IsPrivate);
                        break;
                    case PrivateType.All:
                    default:
                        break;
                }

                if (request.IncludeTags)
                {
                    queryable = queryable.IncludeTags();
                }

                if (request.IncludeUser)
                {
                    queryable = queryable.Include(e => e.User);
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