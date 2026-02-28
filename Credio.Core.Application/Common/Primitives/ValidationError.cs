using Credio.Core.Application.Enums;

namespace Credio.Core.Application.Common.Primitives;

public class ValidationError : Error
{
    public ValidationError(Error[] errors)
        : base("Validation Error",
            "One or more validation errors occurred.",
            ErrorType.BadRequest)
    {
        Errors = errors;
    }

    public Error[] Errors { get; set; }
}