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
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Validation.Common;

namespace SampleExam.Features.UserExamResult
{
    public class Create
    {
        public class Request : IRequest<UserExamResultDTOEnvelope>
        {

            public Request(int userExamId)
            {
                this.UserExamId = userExamId;
            }
            public int UserExamId { get; set; }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.UserExamId).Id<Request, int>(errorCodePrefix + "UserExamExam");
            }
        }
        public class Handler : IRequestHandler<Request, UserExamResultDTOEnvelope>
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

            public async Task<UserExamResultDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = currentUserAccessor.GetCurrentUserId();

                var userExam = await this.context.UserExams
                .Include(e => e.Exam).Where(e => e.Id == request.UserExamId && e.UserId == userId && e.Exam.IsPublished == true).FirstOrDefaultAsync();

                if (userExam == null)
                {
                    throw new Exceptions.UserExamNotFoundException();
                }

                if (!userExam.EndedAt.HasValue)
                {
                    await UserExam.UserExamHelper.EndUserExamIfTimeExpired(context, cancellationToken, userExam);
                }
                if (!userExam.EndedAt.HasValue)
                {
                    throw new Exceptions.UserExamNotFoundException();
                }
                var userExamResult = await context.UserExamResults.FindAsync(userExam.Id);
                if (userExamResult != null)
                {
                    return MakeEnvelope(userExamResult);
                }
                userExamResult = new Domain.UserExamResult() { UserExamId = userExam.Id };

                userExamResult.QuestionCount = context.Questions.Where(e => e.ExamId == userExam.ExamId).Count();

                var userExamQuestionAnswers = context.UserExamQuestions.Where(e => e.UserExamId == userExam.Id)
                .Include(e => e.UserExamQuestionAnswers).ToArray();

                foreach (var userExamQuestionAnswer in userExamQuestionAnswers)
                {
                    if (IsRight(userExamQuestionAnswer)) { userExamResult.RightAnswerCount++; }
                    else { userExamResult.WrongAnswerCount++; }
                }

                userExamResult.IsPassed = (float)userExamResult.RightAnswerCount / (float)userExamResult.QuestionCount * 100
                 >= userExam.Exam.PassPercentage;

                await context.UserExamResults.AddAsync(userExamResult);
                await context.SaveChangesAsync();
                context.Entry(userExamResult).Reload();
                return MakeEnvelope(userExamResult);
            }

            private bool IsRight(UserExamQuestion userExamQuestionAnswer)
            {
                var userAnswers = userExamQuestionAnswer.UserExamQuestionAnswers.Select(e => e.AnswerOptionId).OrderBy(e => e).ToArray();
                var rightAnswers = context.AnswerOptions
                .Where(e => e.QuestionId == userExamQuestionAnswer.QuestionId && e.IsRight == true).Select(e => e.Id).OrderBy(e => e).ToArray();
                return userAnswers.Length == rightAnswers.Length && userAnswers.SequenceEqual(rightAnswers);
            }

            private UserExamResultDTOEnvelope MakeEnvelope(Domain.UserExamResult userExamResult)
            {
                var userExamResultDto = mapper.Map<Domain.UserExamResult, UserExamResultDTO>(userExamResult);
                return new UserExamResultDTOEnvelope(userExamResultDto);
            }
        }
    }
}