using System.Net;
using SampleExam.Common;
using SampleExam.Infrastructure.Errors;

namespace SampleExam.Features.Values
{
    public class Exceptions
    {
        public class ValueNotFoundException : RestException
        {
            public ValueNotFoundException() : base(HttpStatusCode.NotFound, nameof(Domain.Value), Constants.NOT_FOUND)
            {
            }
        }
    }
}