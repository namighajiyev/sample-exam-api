using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using SampleExam.Common;
 
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Security;
using SampleExam.Infrastructure.Validation.Exam;

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

            public IEnumerable<string> Tags { get; set; }

        }


        public class Request : IRequest<ExamDTOEnvelope>
        {
            public ExamData Exam { get; set; }
        }



        public class ExamDataValidator : AbstractValidator<ExamData>
        {
            public ExamDataValidator()
            {
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.Title).ExamTitle<ExamData, string>(errorCodePrefix);
                RuleFor(x => x.Description).ExamDescription<ExamData, string>(errorCodePrefix);
                RuleFor(x => x.TimeInMinutes).ExamTimeInMinutes<ExamData, int>(errorCodePrefix);
                RuleFor(x => x.PassPercentage).ExamPassPercentage<ExamData, int>(errorCodePrefix);
                RuleFor(x => x.IsPrivate).ExamIsPrivate<ExamData, bool>(errorCodePrefix);
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
                var tags = await context.SaveTagsAsync(request.Exam.Tags ?? Enumerable.Empty<string>(), cancellationToken);

                var exam = mapper.Map<ExamData, Domain.Exam>(request.Exam);
                exam.UserId = this.currentUserAccessor.GetCurrentUserId();
                var utcNow = DateTime.UtcNow;
                exam.CreatedAt = utcNow;
                exam.UpdatedAt = utcNow;

                await this.context.Exams.AddAsync(exam, cancellationToken);

                await context.ExamTags.AddRangeAsync(tags.Select(tag => new Domain.ExamTag()
                {
                    Exam = exam,
                    Tag = tag
                }), cancellationToken);

                await context.SaveChangesAsync(cancellationToken);
                var examDto = mapper.Map<Domain.Exam, ExamDTO>(exam);
                return new ExamDTOEnvelope(examDto);
            }

        }
    }
}