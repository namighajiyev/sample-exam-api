using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SampleExam.Infrastructure.Errors
{
    public static class ExceptionHandler
    {
        public static Task HandleException(HttpContext context)
        {

            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = errorFeature.Error;

            var problemDetails = new ValidationProblemDetails()
            {
                Title = "An unexpected error occurred!",
                Status = 500,
                Detail = exception.Message
            };

            switch (exception)
            {
                case RestException re:
                    problemDetails.Title = re.Caption;
                    problemDetails.Detail = re.Error;
                    problemDetails.Status = (int)re.Code;
                    problemDetails.Errors.Add(re.Caption, new string[] { re.Error });
                    context.Response.StatusCode = (int)re.Code;
                    break;
            }

            context.Response.ContentType = "application/problem+json";

            // log the exception etc..
            var responceText = JsonConvert.SerializeObject(problemDetails);
            return context.Response.WriteAsync(responceText);
        }
    }
}