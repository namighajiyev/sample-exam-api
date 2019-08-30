using System;
using System.Net;

namespace SampleExam.Infrastructure.Errors
{
    //https://www.strathweb.com/2018/07/centralized-exception-handling-and-request-validation-in-asp-net-core/
    //https://www.strathweb.com/2018/02/exploring-the-apicontrollerattribute-and-its-features-for-asp-net-core-mvc-2-1/
    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, string caption, string error)
        {
            Code = code;
            Caption = caption;
            Error = error;
        }

        public HttpStatusCode Code { get; }

        public string Caption { get; set; }

        public string Error { get; set; }


    }
}