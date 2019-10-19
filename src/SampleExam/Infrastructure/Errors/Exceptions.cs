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

        public class UserExamNotFoundException : RestException
        {
            public UserExamNotFoundException() : base(HttpStatusCode.NotFound, nameof(Domain.UserExam),
            Constants.NOT_FOUND)
            {
            }
        }

        public class UserExamAlreadyEndedException : RestException
        {
            public UserExamAlreadyEndedException() : base(HttpStatusCode.BadRequest, nameof(Domain.UserExam),
            "Already ended the user exam")
            {
            }
        }
    }
}
