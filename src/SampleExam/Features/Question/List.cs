using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;
 
using SampleExam.Common;
using SampleExam.Infrastructure.Errors;
using FluentValidation;
using SampleExam.Infrastructure.Data;

namespace SampleExam.Features.Question
{
    public class List
    {

        public class Query : IRequest<QuestionsDTOEnvelope>
        {
            public Query(
                    int examId,
                    int? limit,
                    int? offset,
                    bool? includeAnswerOptions
                 )
            {
                this.ExamId = examId;
                this.Limit = limit ?? Constants.FETCH_LIMIT;
                this.Offset = offset ?? Constants.FETCH_OFFSET;
                this.IncludeAnswerOptions = includeAnswerOptions ?? false;
            }

            public int ExamId { get; }
            public int Limit { get; }
            public int Offset { get; }
            public bool IncludeAnswerOptions { get; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(q => q.ExamId)
                .NotNull()
                .WithErrorCode("QuestionsListQueryExamIdNotNull")
                .GreaterThan(0)
                .WithErrorCode("QuestionsListQueryExamIdGreaterThan");
            }

        }

        public class QueryHandler : IRequestHandler<Query, QuestionsDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;

            public QueryHandler(IMapper mapper,
             SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<QuestionsDTOEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {

                var examCount = context.Exams.PublishedAndNotPrivate().Where(e => e.Id == request.ExamId).Count();
                if (examCount == 0)
                {
                    throw new Exceptions.ExamNotFoundException();
                }

                var queryable = context.Questions.AsNoTracking();

                queryable = queryable.Where(e => e.ExamId == request.ExamId);

                if (request.IncludeAnswerOptions)
                {
                    queryable = queryable.Include(e => e.AnswerOptions);
                }

                var questions = await queryable
                .OrderByDescending(e => e.CreatedAt)
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

                var questionsCount = await queryable.CountAsync();

                var questionDTOs = mapper.Map<List<Domain.Question>, List<QuestionDTO>>(questions);

                return new QuestionsDTOEnvelope(questionDTOs, questionsCount);
            }
        }

    }
}