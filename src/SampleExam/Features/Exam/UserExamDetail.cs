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
    public class UserExamDetail
    {
        public class Query : IRequest<ExamDTOEnvelope>
        {
            public Query(
            int id,
            bool? includeTags,
            bool? includeUser
            )
            {
                this.Id = id;
                this.IncludeTags = includeTags ?? false;
                this.IncludeUser = includeUser ?? false;
            }
            public int Id { get; }
            public bool IncludeTags { get; }
            public bool IncludeUser { get; }
        }

        public class QueryHandler : IRequestHandler<Query, ExamDTOEnvelope>
        {
            private IMapper mapper;
            private ICurrentUserAccessor currentUserAccessor;
            private SampleExamContext context;

            public QueryHandler(IMapper mapper, ICurrentUserAccessor currentUserAccessor, SampleExamContext context)
            {
                this.mapper = mapper;
                this.currentUserAccessor = currentUserAccessor;
                this.context = context;
            }
            public async Task<ExamDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                int userId = currentUserAccessor.GetCurrentUserId();

                var queryable = context.Exams.AsNoTracking();

                queryable = queryable.Where(e => e.UserId == userId);


                if (request.IncludeTags)
                {
                    queryable = queryable.IncludeTags();
                }

                if (request.IncludeUser)
                {
                    queryable = queryable.Include(e => e.User);
                }


                var exam = await queryable
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

                if (exam == null)
                {
                    throw new Exceptions.ExamNotFoundException();
                }

                var examDTO = mapper.Map<Domain.Exam, ExamDTO>(exam);

                return new ExamDTOEnvelope(examDTO);
            }
        }


    }
}