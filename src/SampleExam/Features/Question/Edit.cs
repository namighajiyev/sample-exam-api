using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Validation.Common;
using SampleExam.Infrastructure.Validation.Question;

namespace SampleExam.Features.Question
{
    public class Edit
    {
        public class AnswerData
        {
            public int? Id { get; set; }
            public char Key { get; set; }

            public string Text { get; set; }

            public bool IsRight { get; set; }
        }

        public class QuestionData
        {
            public int Id { get; set; }
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
                var errorCodePrefix = nameof(Edit);
                RuleFor(x => x.Id).Id<QuestionData, string>(errorCodePrefix + "Question");
                RuleFor(x => x.Text).QuestionText<QuestionData, string>(errorCodePrefix).When(x => x.Text != null);
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
                RuleFor(x => x.Text).AnswerText<AnswerData, string>(errorCodePrefix).When(x => x.Text != null);
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
                var userId = currentUserAccessor.GetCurrentUserId();

                var question = context.Questions.Where(q => q.Id == request.Question.Id)
                .Include(q => q.AnswerOptions).FirstOrDefault();
                if (question == null)
                {
                    throw new Exceptions.QuestionNotFoundException();
                }
                var examId = question.ExamId;
                var exam = context.Exams.NotPublishedByIdAndUserId(examId, userId).FirstOrDefault();
                if (exam == null)
                {
                    throw new Exceptions.ExamNotFoundException();
                }


                var answerOptions = question.AnswerOptions;
                var answers = request.Question.Answers;
                var answerOptionsToAdd = answers
                .Where(e => !e.Id.HasValue).Select(e => mapper.Map<AnswerData, Domain.AnswerOption>(e)).ToArray();
                var answerOptionsToDelete = answerOptions.Where(ao => !answers.Any(a => a.Id == ao.Id)).ToArray();
                var answerOptionsToUpdate = //answerOptions.Where(ao => answers.Any(a => a.Id == ao.Id)).ToArray();
                (from answerOption in answerOptions
                 join answer in answers on answerOption.Id equals answer.Id
                 where answer.Id != null
                 select new { answerOption = answerOption, answer = answer }).ToArray();


                var utcNow = DateTime.UtcNow;

                question.Text = request.Question.Text ?? question.Text;

                if (context.IsModified(question) || answerOptionsToAdd.Length > 0
                || answerOptionsToDelete.Length > 0 || answerOptionsToUpdate.Length > 0)
                {
                    question.UpdatedAt = utcNow;
                }

                foreach (var answerOption in answerOptionsToAdd)
                {
                    answerOption.QuestionId = question.Id;
                    answerOption.CreatedAt = utcNow;
                    answerOption.UpdatedAt = utcNow;
                }
                foreach (var answerOptionItem in answerOptionsToUpdate)
                {
                    var answer = answerOptionItem.answer;
                    var answerOption = answerOptionItem.answerOption;
                    answerOption.Text = answer.Text ?? answerOption.Text;
                    answerOption.Key = answer.Key;
                    answerOption.IsRight = answer.IsRight;
                    if (context.IsModified(answerOption))
                    {
                        answerOption.UpdatedAt = utcNow;
                    }
                }

                await context.AnswerOptions.AddRangeAsync(answerOptionsToAdd, cancellationToken);
                context.AnswerOptions.RemoveRange(answerOptionsToDelete);
                await context.SaveChangesAsync();
                var questionDto = mapper.Map<Domain.Question, QuestionDTO>(question);
                return new QuestionDTOEnvelope(questionDto);
            }

        }
    }
}