using System.Collections.Generic;
using FluentValidation.Results;

namespace SampleExam.Infrastructure.Errors
{
    public interface IValidationFailuresSerializer
    {
        string Serialize(IEnumerable<ValidationFailure> Errors);
    }
}