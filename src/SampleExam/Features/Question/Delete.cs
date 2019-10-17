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

namespace SampleExam.Features.Question
{
    public class Delete
    {
        public class Request : IRequest<QuestionDTOEnvelope>
        {
            public Request(int id)
            {
                this.Id = id;
            }

            public int Id { get; }
        }


        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                var errorCodePrefix = nameof(Delete);
                RuleFor(x => x.Id).Id<Request, int>(errorCodePrefix + "Question");
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
                var question = context.Questions.Where(q => q.Id == request.Id)
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

                context.AnswerOptions.RemoveRange(question.AnswerOptions);
                context.Questions.Remove(question);
                await context.SaveChangesAsync();
                var questionDto = mapper.Map<Domain.Question, QuestionDTO>(question);
                return new QuestionDTOEnvelope(questionDto);
            }
        }
    }
}
