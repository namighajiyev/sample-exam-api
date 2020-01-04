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
            Constants.NOT_FOUND, new Error(nameof(QuestionNotFoundException), Constants.NOT_FOUND))
            {
            }
        }

        public class ExamNotFoundException : RestException
        {
            public ExamNotFoundException() : base(HttpStatusCode.NotFound, nameof(Domain.Exam),
            Constants.NOT_FOUND, new Error(nameof(ExamNotFoundException), Constants.NOT_FOUND))
            {
            }
        }

        public class UserExamNotFoundException : RestException
        {
            public UserExamNotFoundException() : base(HttpStatusCode.NotFound, nameof(Domain.UserExam),
            Constants.NOT_FOUND, new Error(nameof(UserExamNotFoundException), Constants.NOT_FOUND))
            {
            }
        }

        public class UserExamResultNotFoundException : RestException
        {
            public UserExamResultNotFoundException() : base(HttpStatusCode.NotFound, nameof(Domain.UserExam),
            Constants.NOT_FOUND, new Error(nameof(UserExamResultNotFoundException), Constants.NOT_FOUND))
            {
            }
        }

        public class AnswerOptionNotFoundException : RestException
        {
            public AnswerOptionNotFoundException() : base(HttpStatusCode.NotFound, nameof(Domain.AnswerOption),
            Constants.NOT_FOUND, new Error(nameof(AnswerOptionNotFoundException), Constants.NOT_FOUND))
            {
            }

            public AnswerOptionNotFoundException(int answerOptionId) : this()
            {
                this.Extensions.Add(nameof(answerOptionId), answerOptionId);
            }
        }

        public class UserExamAlreadyEndedException : RestException
        {
            public UserExamAlreadyEndedException() : base(HttpStatusCode.BadRequest, nameof(Domain.UserExam),
            "Already ended the user exam", new Error(nameof(UserExamAlreadyEndedException), "Already ended the user exam"))
            {
            }
        }

        public class AnswerToRadioQuestionFormatException : RestException
        {
            public AnswerToRadioQuestionFormatException() : base(HttpStatusCode.BadRequest, nameof(Domain.UserExam),
            "Radio type question must only have single user answer", new Error(nameof(AnswerToRadioQuestionFormatException), "Radio type question must only have single user answer"))
            {
            }
        }

        public class PrivateUserExamEditException : RestException
        {
            public PrivateUserExamEditException() : base(HttpStatusCode.BadRequest, nameof(Domain.UserExam),
            "Private user exam can only be edited by creator of exam", new Error(nameof(PrivateUserExamEditException),
           "Private user exam can only be edited by creator of exam"))
            {
            }
        }

        public class InvalidAnswerOptionExamException : RestException
        {
            public InvalidAnswerOptionExamException() : base(HttpStatusCode.BadRequest, nameof(Domain.UserExam),
            "Answer option is not for this exam", new Error(nameof(InvalidAnswerOptionExamException),
            "Answer option is not for this exam"))
            {
            }

            public InvalidAnswerOptionExamException(int answerOptionId) : this()
            {
                this.Extensions.Add(nameof(answerOptionId), answerOptionId);
            }
        }

    }
}
