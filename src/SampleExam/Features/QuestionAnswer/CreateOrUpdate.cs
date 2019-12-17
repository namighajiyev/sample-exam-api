using System;
using System.Collections.Generic;
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
            public int QuestionId { get; set; }
            public IEnumerable<int> AnswerOptionIds { get; set; }
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
                RuleFor(x => x.AnswerOptionIds).NotEmptyEnumerable<QuestionAnswerData, int>(errorCodePrefix);
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
                var answerOptionIds = requestQA.AnswerOptionIds.ToArray();
                var userExam = await this.context.UserExams.Include(e => e.Exam).Where(e => e.Id == requestQA.UserExamId).FirstOrDefaultAsync();
                if (userExam == null)
                {
                    throw new UserExamNotFoundException();
                }

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
                var question = await context.Questions.FindAsync(requestQA.QuestionId);
                if (question.QuestionTypeId == SeedData.QuestionTypes.Radio.Id && answerOptionIds.Count() > 1)
                {
                    throw new RadioQuestionWithMultipleAnswerException();
                }

                var userExamQuestion = await this.context.UserExamQuestions
                                            .Where(e => e.QuestionId == requestQA.QuestionId
                                            && e.UserExamId == requestQA.UserExamId).Include(e => e.UserExamQuestionAnswers)
                                            .FirstOrDefaultAsync(cancellationToken);
                var isCreate = userExamQuestion == null;
                var utcNow = DateTime.UtcNow;
                userExamQuestion = userExamQuestion ?? new UserExamQuestion()
                {
                    UserExamId = requestQA.UserExamId,
                    QuestionId = requestQA.QuestionId,
                    //UserExamQuestionAnswers = new List<UserExamQuestionAnswr>(),
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                };

                var isUnchanged = !isCreate && userExamQuestion.UserExamQuestionAnswers
                .Select(e => e.AnswerOptionId).OrderBy(e => e).ToArray()
                .SequenceEqual(answerOptionIds.OrderBy(e => e).ToArray());
                if (isUnchanged)
                {
                    return MakeEnvelope(userExamQuestion);
                }
                //validation
                foreach (var answerOptionId in answerOptionIds)
                {
                    var answerOption = await this.context.AnswerOptions.Include(e => e.Question).Where(e => e.Id == answerOptionId).FirstOrDefaultAsync(cancellationToken);

                    if (answerOption == null)
                    {
                        throw new AnswerOptionNotFoundException() { Extensions = new Dictionary<string, object> { { nameof(answerOptionId), answerOptionId } } };
                    };

                    if (answerOption.Question.ExamId != userExam.ExamId)
                    {
                        new InvalidAnswerOptionExamException() { Extensions = new Dictionary<string, object> { { nameof(answerOptionId), answerOptionId } } };
                    }

                }

                if (isCreate)
                {
                    var answers = answerOptionIds.Select(answerOptionId => new UserExamQuestionAnswr()
                    {
                        UserExamId = requestQA.UserExamId,
                        QuestionId = requestQA.QuestionId,
                        AnswerOptionId = answerOptionId,
                        CreatedAt = utcNow,
                        UpdatedAt = utcNow
                    }).ToList();
                    await this.context.UserExamQuestions.AddAsync(userExamQuestion);
                    await this.context.UserExamQuestionAnswers.AddRangeAsync(answers);
                }
                else
                {
                    var answersToDelete = userExamQuestion.UserExamQuestionAnswers.Where(qa => !answerOptionIds.Any(id => id == qa.AnswerOptionId)).ToArray();
                    var answersToAdd = answerOptionIds.Where(id => !userExamQuestion.UserExamQuestionAnswers.Any(qa => qa.AnswerOptionId == id))
                    .Select(answerOptionId => new UserExamQuestionAnswr()
                    {
                        UserExamId = requestQA.UserExamId,
                        QuestionId = requestQA.QuestionId,
                        AnswerOptionId = answerOptionId,
                        CreatedAt = utcNow,
                        UpdatedAt = utcNow
                    });
                    userExamQuestion.UpdatedAt = utcNow;
                    this.context.UserExamQuestionAnswers.RemoveRange(answersToDelete);
                    await this.context.UserExamQuestionAnswers.AddRangeAsync(answersToAdd);
                }
                await context.SaveChangesAsync(cancellationToken);
                return MakeEnvelope(userExamQuestion);
            }

            private QuestionAnswerDTOEnvelope MakeEnvelope(UserExamQuestion userExamQuestion)
            {
                var questionAnswerDto = mapper.Map<Domain.UserExamQuestion, QuestionAnswerDTO>(userExamQuestion);
                return new QuestionAnswerDTOEnvelope(questionAnswerDto);
            }
        }
    }
}