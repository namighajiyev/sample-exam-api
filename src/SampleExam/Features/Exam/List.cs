using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;
 
using SampleExam.Common;
using SampleExam.Infrastructure.Data;

namespace SampleExam.Features.Exam
{
    public class List
    {

        public class Query : IRequest<ExamsDTOEnvelope>
        {
            public Query(
                    int? userId,
                    int? limit,
                    int? offset,
                    bool? includeTags,
                    bool? includeUser
                 )
            {
                this.UserId = userId;
                this.Limit = limit ?? Constants.FETCH_LIMIT;
                this.Offset = offset ?? Constants.FETCH_OFFSET;
                this.IncludeTags = includeTags ?? false;
                this.IncludeUser = includeUser ?? false;

            }

            public int? UserId { get; }
            public int Limit { get; }
            public int Offset { get; }
            public bool IncludeTags { get; }
            public bool IncludeUser { get; }
        }
        public class QueryHandler : IRequestHandler<Query, ExamsDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;

            public QueryHandler(IMapper mapper,
             SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<ExamsDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {

                var queryable = context.Exams.AsNoTracking();

                queryable = queryable.PublishedAndNotPrivate();

                if (request.UserId.HasValue)
                {
                    queryable = queryable.Where(e => e.UserId == request.UserId.Value);
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