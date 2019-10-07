using System.Linq;
using System.Web;
using System.Net;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SampleExam.Common;

namespace SampleExam.Infrastructure.Errors
{
    public class ApiValidatorInterceptor : IValidatorInterceptor
    {

        public FluentValidation.ValidationContext BeforeMvcValidation(ControllerContext controllerContext, FluentValidation.ValidationContext validationContext)
        {
            return validationContext;
        }




        public FluentValidation.Results.ValidationResult AfterMvcValidation(ControllerContext controllerContext,
        FluentValidation.ValidationContext validationContext, FluentValidation.Results.ValidationResult result)
        {

            if (result.Errors.Count == 0) { return result; }

            var problemDetails = new ApiProblemDetails();
            foreach (var failure in result.Errors)
            {
                var propName = failure.PropertyName;

                if (!problemDetails.Errors.ContainsKey(propName))
                {
                    problemDetails.Errors.Add(propName, new List<Error>());
                }
                problemDetails.Errors[propName].Add(new Error(failure.ErrorCode, failure.ErrorMessage));
            }
            var context = controllerContext.HttpContext;
            problemDetails.Title = "One or more validation errors occurred.";
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

            var errors = JsonConvert.SerializeObject(problemDetails);

            context.Items[Constants.VALIDATION_ERRORS_KEY] = errors;
            return new ValidationResult();
        }
    }
}