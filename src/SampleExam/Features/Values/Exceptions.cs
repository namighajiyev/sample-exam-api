using System.Net;
using SampleExam.Common;
using SampleExam.Infrastructure.Errors;

namespace SampleExam.Features.Values
{
    public static class Exceptions
    {
        public static readonly RestException ValueNotFoundException =
        new RestException(HttpStatusCode.NotFound, "Value", Constants.NOT_FOUND);
    }
}