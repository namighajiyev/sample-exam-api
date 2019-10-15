using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using SampleExam.Infrastructure;

namespace SampleExam.Features.Question
{
    public class Create
    {

        public class AnswerData
        {
            public char Key { get; set; }

            public string Text { get; set; }

            public bool IsRight { get; set; }
        }

        public class QuestionData
        {
            public string Text { get; set; }

            public IEnumerable<AnswerData> Answers { get; set; }
        }
        public class Request : IRequest<QuestionDTOEnvelope>
        {
            public QuestionData Question { get; set; }
        }

        public class QuestionDataValidator : AbstractValidator<QuestionData>
        {
            public QuestionDataValidator()
            {
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.Text).QuestionText<QuestionData, string>(errorCodePrefix);
                RuleFor(x => x.Answers).QuestionAnswers<QuestionData, AnswerData>((e) => e.Key, errorCodePrefix);
            }
        }

        public class AnswerDataValidator : AbstractValidator<AnswerData>
        {
            public AnswerDataValidator()
            {
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.Key).AnswerKey<AnswerData, char>(errorCodePrefix);
                RuleFor(x => x.Text).AnswerText<AnswerData, string>(errorCodePrefix);
                RuleFor(x => x.IsRight).AnswerIsRight<AnswerData, bool>(errorCodePrefix);
            }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Question).NotNull().SetValidator(new QuestionDataValidator());
                RuleFor(x => x.Question.Answers).NotNull();
                RuleForEach(x => x.Question.Answers).SetValidator(new AnswerDataValidator());
            }
        }

        public class Handler : IRequestHandler<Request, QuestionDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;
            private readonly ICurrentUserAccessor currentUserAccessor;

            public Handler(IMapper mapper, SampleExamContext context, ICurrentUserAccessor currentUserAccessor)
            {
                this.mapper = mapper;
                this.context = context;
                this.currentUserAccessor = currentUserAccessor;
            }

            public async Task<QuestionDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                // var tags = await context.SaveTagsAsync(request.Exam.Tags ?? Enumerable.Empty<string>(), cancellationToken);

                // var exam = mapper.Map<ExamData, Domain.Exam>(request.Exam);
                // exam.UserId = this.currentUserAccessor.GetCurrentUserId();
                // var utcNow = DateTime.UtcNow;
                // exam.CreatedAt = utcNow;
                // exam.UpdatedAt = utcNow;

                // await this.context.Exams.AddAsync(exam, cancellationToken);

                // await context.ExamTags.AddRangeAsync(tags.Select(tag => new Domain.ExamTag()
                // {
                //     Exam = exam,
                //     Tag = tag
                // }), cancellationToken);

                // await context.SaveChangesAsync(cancellationToken);
                // var examDto = mapper.Map<Domain.Exam, ExamDTO>(exam);
                // return new QuestionDTOEnvelope(examDto);

                await Task.CompletedTask;
                return new QuestionDTOEnvelope(new QuestionDTO());
            }

        }
    }
}