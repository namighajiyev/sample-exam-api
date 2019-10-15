using System.Collections.Generic;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SampleExam.Infrastructure.Errors
{
    public class ValidationFailuresSerializer : IValidationFailuresSerializer
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ValidationFailuresSerializer(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public string Serialize(IEnumerable<ValidationFailure> Errors)
        {
            var problemDetails = new ApiProblemDetails();
            foreach (var failure in Errors)
            {
                var propName = failure.PropertyName;

                if (!problemDetails.Errors.ContainsKey(propName))
                {
                    problemDetails.Errors.Add(propName, new List<Error>());
                }
                problemDetails.Errors[propName].Add(new Error(failure.ErrorCode, failure.ErrorMessage));
            }

            problemDetails.Title = "One or more validation errors occurred.";
            problemDetails.Status = (int)System.Net.HttpStatusCode.BadRequest;
            problemDetails.Extensions.Add("traceId", httpContextAccessor.HttpContext.TraceIdentifier);

            var errors = JsonConvert.SerializeObject(problemDetails);
            return errors;
        }
    }
}