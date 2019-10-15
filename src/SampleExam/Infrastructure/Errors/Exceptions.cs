using System.Net;
using SampleExam.Common;

namespace SampleExam.Infrastructure.Errors
{
    public class Exceptions
    {
        public class QuestionNotFoundException : RestException
        {
            public QuestionNotFoundException() : base(HttpStatusCode.NotFound,
            nameof(Domain.Question),
            Constants.NOT_FOUND)
            {
            }
        }

        public class ExamNotFoundException : RestException
        {
            public ExamNotFoundException() : base(HttpStatusCode.NotFound, nameof(Domain.Exam),
            Constants.NOT_FOUND)
            {
            }
        }
    }
}
