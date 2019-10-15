using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure;
using SampleExam.Common;
using System.Linq;
using SampleExam.Infrastructure.Errors;

namespace SampleExam.Features.Exam
{
    public class Edit
    {
        public class ExamData
        {
            internal int Id { get; set; }
            public string Title { get; set; }

            public string Description { get; set; }

            public int? TimeInMinutes { get; set; }

            public int? PassPercentage { get; set; }

            public bool? IsPrivate { get; set; }

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
                var errorCodePrefix = nameof(Edit);
                RuleFor(x => x.Title).ExamTitle<ExamData, string>(errorCodePrefix).When(x => x.Title != null);
                RuleFor(x => x.Description).ExamDescription<ExamData, string>(errorCodePrefix).When(x => x.Description != null);
                RuleFor(x => x.TimeInMinutes.GetValueOrDefault()).ExamTimeInMinutes<ExamData, int>(errorCodePrefix).When(x => x.TimeInMinutes.HasValue);
                RuleFor(x => x.PassPercentage.GetValueOrDefault()).ExamPassPercentage<ExamData, int>(errorCodePrefix).When(x => x.PassPercentage.HasValue);
                RuleFor(x => x.IsPrivate.GetValueOrDefault()).ExamIsPrivate<ExamData, bool>(errorCodePrefix).When(x => x.IsPrivate.HasValue);
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

                var userId = currentUserAccessor.GetCurrentUserId();
                var examData = request.Exam;
                var exam = await context.Exams.NotPublishedByIdAndUserId(examData.Id, userId)
                .IncludeTags()
                .FirstOrDefaultAsync(cancellationToken);
                if (exam == null)
                {
                    throw new Exceptions.ExamNotFoundException();
                }

                exam.Title = examData.Title ?? exam.Title;
                exam.Description = examData.Description ?? exam.Description;
                exam.TimeInMinutes = examData.TimeInMinutes ?? exam.TimeInMinutes;
                exam.PassPercentage = examData.PassPercentage ?? exam.PassPercentage;
                exam.IsPrivate = examData.IsPrivate ?? exam.IsPrivate;
                var examTagsToAdd = Array.Empty<Domain.ExamTag>();
                var examTagsToDelete = Array.Empty<Domain.ExamTag>();
                if (request.Exam.Tags != null)
                {
                    var tags = await context.SaveTagsAsync(request.Exam.Tags ?? Enumerable.Empty<string>(), cancellationToken);
                    var examTags = exam.ExamTags;
                    examTagsToDelete = examTags.Where(et => !tags.Any(t => t.TagId == et.TagId)).ToArray();
                    examTagsToAdd = tags.Where(t => !examTags.Any(et => et.TagId == t.TagId))
                   .Select(t => new Domain.ExamTag() { Tag = t, TagId = t.TagId, Exam = exam, ExamId = exam.Id }).ToArray();
                }

                if (context.IsModified(exam) || examTagsToAdd.Length > 0 || examTagsToDelete.Length > 0)
                {
                    exam.UpdatedAt = DateTime.UtcNow;
                }

                await context.ExamTags.AddRangeAsync(examTagsToAdd, cancellationToken);
                context.ExamTags.RemoveRange(examTagsToDelete);

                await context.SaveChangesAsync(cancellationToken);
                var examDto = mapper.Map<Domain.Exam, ExamDTO>(exam);
                return new ExamDTOEnvelope(examDto);
            }
        }
    }

}