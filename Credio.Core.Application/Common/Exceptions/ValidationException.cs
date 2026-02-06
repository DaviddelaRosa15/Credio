using Credio.Core.Application.Common.Primitives;
using FluentValidation.Results;

namespace Credio.Core.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public Error[] Failures { get; }

    public ValidationException(ValidationFailure[] failures)
        : base("One or more validations errors happen")
    {
        Failures = CreateValidationError(failures);
    }

    private static Error[] CreateValidationError(IEnumerable<ValidationFailure> failures)
        => failures.Select(x => Error.Validation(x.ErrorMessage)).ToArray();
}