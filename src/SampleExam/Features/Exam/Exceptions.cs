using System.Net;
using SampleExam.Common;
using SampleExam.Infrastructure.Errors;

namespace SampleExam.Features.Exam
{
    public class Exceptions
    {
        public class ExamNotFoundException : RestException
        {
            public ExamNotFoundException() : base(HttpStatusCode.NotFound, nameof(Domain.Exam),
            Constants.NOT_FOUND)
            {
            }
        }
    }
}