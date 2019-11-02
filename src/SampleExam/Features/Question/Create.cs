using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Validation.Common;
using SampleExam.Infrastructure.Validation.Question;

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
            [JsonIgnore]
            public int ExamId { get; set; }
            public QuestionData Question { get; set; }
        }

        public class QuestionDataValidator : AbstractValidator<QuestionData>
        {
            public QuestionDataValidator()
            {
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.Text).QuestionText<QuestionData, string>(errorCodePrefix);
                RuleFor(x => x.Answers).QuestionAnswers<QuestionData, AnswerData>
                                    (e => e.Key, e => e.IsRight, errorCodePrefix);
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
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.ExamId).Id<Request, int>(errorCodePrefix + "QuestionExam");
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
                var userId = currentUserAccessor.GetCurrentUserId();
                var exam = context.Exams.NotPublishedByIdAndUserId(request.ExamId, userId).FirstOrDefault();
                if (exam == null)
                {
                    throw new Exceptions.ExamNotFoundException();
                }

                var question = mapper.Map<QuestionData, Domain.Question>(request.Question);
                question.ExamId = exam.Id;
                question.Exam = exam;
                var utcNow = DateTime.UtcNow;
                question.CreatedAt = utcNow;
                question.UpdatedAt = utcNow;

                question.AnswerOptions.ToList().ForEach(ao =>
                {
                    ao.Question = question;
                    ao.CreatedAt = utcNow;
                    ao.UpdatedAt = utcNow;
                });
                await this.context.Questions.AddAsync(question);
                await context.SaveChangesAsync();
                var questionDto = mapper.Map<Domain.Question, QuestionDTO>(question);
                return new QuestionDTOEnvelope(questionDto);
            }

        }
    }
}