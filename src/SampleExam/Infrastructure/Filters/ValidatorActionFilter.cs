using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SampleExam.Common;

namespace SampleExam.Infrastructure.Filters
{
    public class ValidatorActionFilter : IActionFilter
    {
        private readonly ILogger logger;

        public ValidatorActionFilter(ILogger<ValidatorActionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var validationErrors = (string)filterContext.HttpContext.Items[Constants.VALIDATION_ERRORS_KEY];
            if (!string.IsNullOrWhiteSpace(validationErrors))
            {
                var result = new ContentResult();
                result.Content = validationErrors;
                result.ContentType = "application/json";
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                filterContext.Result = result;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}