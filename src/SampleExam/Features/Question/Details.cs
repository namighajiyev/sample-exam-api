using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Common;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Validation.Common;

namespace SampleExam.Features.Question
{
    public class Details
    {
        public class Query : IRequest<QuestionDTOEnvelope>
        {
            public Query(
                    bool isAuthorized,
                    int id,
                    int? limit,
                    int? offset,
                    bool? includeAnswerOptions
                 )
            {
                this.IsAuthorized = isAuthorized;
                this.Id = id;
                this.Limit = limit ?? Constants.FETCH_LIMIT;
                this.Offset = offset ?? Constants.FETCH_OFFSET;
                this.IncludeAnswerOptions = includeAnswerOptions ?? false;
            }

            public bool IsAuthorized { get; private set; }
            public int Id { get; }
            public int Limit { get; }
            public int Offset { get; }
            public bool IncludeAnswerOptions { get; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                var errorCodePrefix = nameof(Details);
                RuleFor(q => q.Id).Id<Query, int>(errorCodePrefix + "ExamQuestion");
            }

        }

        public class QueryHandler : IRequestHandler<Query, QuestionDTOEnvelope>
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
            public async Task<QuestionDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = context.Questions.AsNoTracking();
                queryable = queryable.Where(q => q.Id == request.Id);

                if (request.IncludeAnswerOptions)
                {
                    queryable = queryable.Include(e => e.AnswerOptions);
                }
                var question = await queryable.FirstOrDefaultAsync();

                if (question == null)
                {
                    throw new Exceptions.QuestionNotFoundException();
                }
                var examId = question.ExamId;

                int examCount = 0;
                if (request.IsAuthorized)
                {
                    var userId = currentUserAccessor.GetCurrentUserId();
                    examCount = context.Exams.ByIdAndUserId(examId, userId).Count();
                }
                else
                {
                    examCount = context.Exams.PublishedAndNotPrivate().Where(e => e.Id == examId).Count();
                }

                if (examCount == 0)
                {
                    throw new Exceptions.ExamNotFoundException();
                }

                var questionDTO = mapper.Map<Domain.Question, QuestionDTO>(question);

                return new QuestionDTOEnvelope(questionDTO);
            }

        }
    }
}