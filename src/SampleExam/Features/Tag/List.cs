using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Common;
using SampleExam.Infrastructure;

namespace SampleExam.Features.Tag
{
    public class List
    {
        public class Query : IRequest<TagsDTOEnvelope>
        {
            public Query(int? limit, int? offset)
            {
                this.Limit = limit ?? Constants.FETCH_LIMIT;
                this.Offset = offset ?? Constants.FETCH_OFFSET;
            }

            public int Limit { get; }
            public int Offset { get; }
        }
        public class QueryHandler : IRequestHandler<Query, TagsDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;

            public QueryHandler(IMapper mapper, SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<TagsDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = context.Tags.AsNoTracking();
                var tags = await queryable.OrderByDescending(e => e.CreatedAt)
                            .Skip(request.Offset)
                            .Take(request.Limit)
                            .ToListAsync(cancellationToken);

                var tagsList = mapper.Map<List<Domain.Tag>, List<TagDTO>>(tags);
                var tagsCount = queryable.Count();
                return new TagsDTOEnvelope(tagsList, tagsCount);
            }
        }
    }
}