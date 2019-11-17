using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;

namespace SampleExam.Features.UserExam
{
    public class Details
    {
        public class Query : IRequest<UserExamDTOEnvelope>
        {
            public Query(
            int id,
            bool? includeExam
            )
            {
                this.Id = id;
                this.IncludeExam = includeExam ?? false;
            }

            public int Id { get; }
            public bool IncludeExam { get; }
        }

        public class QueryHandler : IRequestHandler<Query, UserExamDTOEnvelope>
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

            public async Task<UserExamDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {

                var userId = currentUserAccessor.GetCurrentUserId();
                var queryable = this.context.UserExams.AsNoTracking().Where(e => e.Id == request.Id && e.UserId == userId);
                if (request.IncludeExam)
                {
                    queryable = queryable.Include(e => e.Exam);
                }

                var userExam = await queryable.FirstOrDefaultAsync();
                if (userExam == null)
                {
                    throw new Exceptions.UserExamNotFoundException();
                }

                var userExamDTO = mapper.Map<Domain.UserExam, UserExamDTO>(userExam);

                return new UserExamDTOEnvelope(userExamDTO);
            }
        }

    }
}