using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Domain;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Validation.Common;
using static SampleExam.Infrastructure.Errors.Exceptions;

namespace SampleExam.Features.QuestionAnswer
{
    public class CreateOrUpdate
    {
        public class QuestionAnswerData
        {
            public int UserExamId { get; set; }
            public int AnswerOptionId { get; set; }
        }

        public class Request : IRequest<QuestionAnswerDTOEnvelope>
        {
            public QuestionAnswerData QuestionAnswer { get; set; }
        }

        public class QuestionAnswerDataValidator : AbstractValidator<QuestionAnswerData>
        {
            public QuestionAnswerDataValidator()
            {
                var errorCodePrefix = nameof(CreateOrUpdate);
                RuleFor(x => x.UserExamId).Id<QuestionAnswerData, int>(errorCodePrefix);
                RuleFor(x => x.AnswerOptionId).Id<QuestionAnswerData, int>(errorCodePrefix);
            }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.QuestionAnswer).NotNull().SetValidator(new QuestionAnswerDataValidator());
            }
        }
        public class Handler : IRequestHandler<Request, QuestionAnswerDTOEnvelope>
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
            public async Task<QuestionAnswerDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var requestQA = request.QuestionAnswer;
                var answerOption = await this.context.AnswerOptions.Include(e => e.Question).Where(e => e.Id == requestQA.AnswerOptionId).FirstOrDefaultAsync(cancellationToken);
                if (answerOption == null)
                {
                    throw new AnswerOptionNotFoundException();
                }

                var userExam = await this.context.UserExams.Include(e => e.Exam).Where(e => e.Id == requestQA.UserExamId).FirstOrDefaultAsync();
                if (userExam == null)
                {
                    throw new UserExamNotFoundException();
                }
                if (answerOption.Question.ExamId != userExam.ExamId)
                {
                    new InvalidAnswerOptionExamException();
                }
                var questionAnswer = await this.context.UserExamQuestionAnswers.Where(e => e.QuestionId == answerOption.QuestionId
                && e.UserExamId == requestQA.UserExamId).FirstOrDefaultAsync(cancellationToken);

                if (userExam.EndedAt.HasValue)
                {
                    throw new UserExamAlreadyEndedException();
                }

                if (userExam.StartedtedAt.AddMinutes(userExam.Exam.TimeInMinutes) <= DateTime.UtcNow)
                {
                    userExam.EndedAt = userExam.StartedtedAt.AddMinutes(userExam.Exam.TimeInMinutes);
                    await context.SaveChangesAsync(cancellationToken);
                    throw new UserExamAlreadyEndedException();
                }


                UserExamQuestionAnswer newQuestionAnswer = null;
                var utcNow = DateTime.UtcNow;
                newQuestionAnswer = new UserExamQuestionAnswer()
                {
                    UserExamId = requestQA.UserExamId,
                    QuestionId = answerOption.QuestionId,
                    AnswerOptionId = requestQA.AnswerOptionId,
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                };

                if (questionAnswer == null)
                {
                    await this.context.UserExamQuestionAnswers.AddAsync(newQuestionAnswer);
                }
                else if (questionAnswer.AnswerOptionId == requestQA.AnswerOptionId)//already this answer selected
                {
                    newQuestionAnswer = questionAnswer;
                }
                else
                {
                    this.context.UserExamQuestionAnswers.Remove(questionAnswer);
                    await this.context.UserExamQuestionAnswers.AddAsync(newQuestionAnswer);
                }


                await context.SaveChangesAsync(cancellationToken);
                var questionAnswerDto = mapper.Map<Domain.UserExamQuestionAnswer, QuestionAnswerDTO>(newQuestionAnswer);
                return new QuestionAnswerDTOEnvelope(questionAnswerDto);
            }

        }
    }
}