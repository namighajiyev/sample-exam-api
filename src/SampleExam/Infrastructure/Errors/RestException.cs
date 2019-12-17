using System;
using System.Collections.Generic;
using System.Net;

namespace SampleExam.Infrastructure.Errors
{
    //https://www.strathweb.com/2018/07/centralized-exception-handling-and-request-validation-in-asp-net-core/
    //https://www.strathweb.com/2018/02/exploring-the-apicontrollerattribute-and-its-features-for-asp-net-core-mvc-2-1/
    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, string title, string detail, Error error)
        {
            Code = code;
            Title = title;
            Detail = detail;
            Error = error;
        }

        public HttpStatusCode Code { get; }

        public string Title { get; set; }

        public string Detail { get; set; }
        public Error Error { get; private set; }

        public IDictionary<string, object> Extensions { get; set; }
    }
}