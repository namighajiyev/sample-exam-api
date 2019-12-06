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

                //check userexam exists
                //check userExam is current users.
                //exam published
                //check exam is ended.
                //get userexam result if exist return that with created false
                //create exam result and return that with is created true

                /////////////////create exam////////////////////////////
                //should calculate : 1) QuestionCount, 2) RightAnswerCount, 3) WrongAnswerCount, 
                //4) NoAnswerCount, 5) IsPassed
                //QuestionCount = get count(all) of  Question with ExamId = userExam.ExamId
                //RightAnswerCount = get count(all) where UserExamId = request.UserExamId && UserExamQuestionAnswer.?foreach&.forall.AnswerOption.IsRight == true

                // var userId = this.currentUserAccessor.GetCurrentUserId();
                // var exam = context.Exams.Where(e => e.Id == request.ExamId && e.IsPublished == true).First();

                // if (exam == null)
                // {
                //     throw new Exceptions.ExamNotFoundException();
                // }
                // if (exam.IsPrivate && exam.UserId != userId)
                // {
                //     throw new Exceptions.ExamNotFoundException();
                // }

                // var userExam = new Domain.UserExam();
                // userExam.ExamId = request.ExamId;
                // userExam.UserId = userId;
                // userExam.StartedtedAt = DateTime.UtcNow;

                // await this.context.UserExams.AddAsync(userExam, cancellationToken);

                // await context.SaveChangesAsync(cancellationToken);
                // var userExamDto = mapper.Map<Domain.UserExam, UserExamDTO>(userExam);
                // return new UserExamDTOEnvelope(userExamDto);
                return null;
            }

        }
    }
}