using System.Collections.Generic;
using FluentValidation.Results;

namespace SampleExam.Infrastructure.Validation
{
    public interface IValidationFailuresSerializer
    {
        string Serialize(IEnumerable<ValidationFailure> Errors);
    }
}