using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using SampleExam.Common;
using SampleExam.Infrastructure;

namespace SampleExam.Features.Exam
{
    public class Create
    {
        public class ExamData
        {
            public string Title { get; set; }

            public string Description { get; set; }

            public int TimeInMinutes { get; set; }

            public int PassPercentage { get; set; }

            public bool IsPrivate { get; set; }

        }


        public class Request : IRequest<ExamDTOEnvelope>
        {
            public ExamData Exam { get; set; }
        }



        public class ExamDataValidator : AbstractValidator<ExamData>
        {
            public ExamDataValidator()
            {
                RuleFor(x => x.Title)
                        .NotNull()
                        .WithErrorCode("CreateExamTitleNotNull")
                        .NotEmpty()
                        .WithErrorCode("CreateExamTitleNotEmpty")
                        .MaximumLength(Constants.EXAM_TITLE_LEN)
                        .WithErrorCode("CreateExamTitleMaximumLength");

                RuleFor(x => x.Description)
                        .NotNull()
                        .WithErrorCode("CreateExamDescriptionNotNull")
                        .NotEmpty()
                        .WithErrorCode("CreateExamDescriptionNotEmpty")
                        .MaximumLength(Constants.EXAM_DESCRIPTIION_LEN)
                        .WithErrorCode("CreateExamDescriptionMaximumLength");

                RuleFor(x => x.TimeInMinutes)
                        .NotNull()
                        .WithErrorCode("CreateExamTimeInMinutesNotNull")
                        .NotEmpty()
                        .WithErrorCode("CreateExamTimeInMinutesNotEmpty")
                        .LessThanOrEqualTo(Constants.EXAM_TIME_IN_MINUTES_MAX)
                        .WithErrorCode("CreateExamTimeInMinutesLessThanOrEqualTo")
                        .GreaterThanOrEqualTo(Constants.EXAM_TIME_IN_MINUTES_MIN)
                        .WithErrorCode("CreateExamTimeInMinutesGreaterThanOrEqualTo");

                RuleFor(x => x.PassPercentage)
                        .NotNull()
                        .WithErrorCode("CreateExamPassPercentageNotNull")
                        .NotEmpty()
                        .WithErrorCode("CreateExamPassPercentageNotEmpty")
                        .LessThanOrEqualTo(Constants.EXAM_PASS_PERCENTAGE_MAX)
                        .WithErrorCode("CreateExamPassPercentageLessThanOrEqualTo")
                        .GreaterThanOrEqualTo(Constants.EXAM_PASS_PERCENTAGE_MIN)
                        .WithErrorCode("CreateExamPassPercentageGreaterThanOrEqualTo");

                RuleFor(x => x.IsPrivate)
                        .NotNull()
                        .WithErrorCode("CreateExamIsPrivateNotNull");

            }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Exam).NotNull().SetValidator(new ExamDataValidator());
            }
        }

        public class Handler : IRequestHandler<Request, ExamDTOEnvelope>
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

            public async Task<ExamDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var exam = mapper.Map<ExamData, Domain.Exam>(request.Exam);
                exam.UserId = this.currentUserAccessor.GetCurrentUserId();
                var utcNow = DateTime.UtcNow;
                exam.CreatedAt = utcNow;
                exam.UpdatedAt = utcNow;
                await this.context.Exams.AddAsync(exam, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                var examDto = mapper.Map<Domain.Exam, ExamDTO>(exam);
                return new ExamDTOEnvelope(examDto);
            }
        }
    }
}