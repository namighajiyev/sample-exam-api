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

namespace SampleExam.Infrastructure.Validation
{
    public class ApiValidatorInterceptor : IValidatorInterceptor
    {
        private readonly IValidationFailuresSerializer serializer;

        public ApiValidatorInterceptor(IValidationFailuresSerializer serializer) => this.serializer = serializer;
        public FluentValidation.ValidationContext BeforeMvcValidation(ControllerContext controllerContext, FluentValidation.ValidationContext validationContext)
        {
            return validationContext;
        }




        public FluentValidation.Results.ValidationResult AfterMvcValidation(ControllerContext controllerContext,
        FluentValidation.ValidationContext validationContext, FluentValidation.Results.ValidationResult result)
        {

            if (result.Errors.Count == 0) { return result; }
            var errors = this.serializer.Serialize(result.Errors);
            var context = controllerContext.HttpContext;
            context.Items[Constants.VALIDATION_ERRORS_KEY] = errors;
            return new ValidationResult();
        }
    }
}