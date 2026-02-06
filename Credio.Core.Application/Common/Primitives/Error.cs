using Credio.Core.Application.Enums;

namespace Credio.Core.Application.Common.Primitives;

public class Error
{
    public string ErrorCode { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    protected Error(
        string errorCode,
        string description,
        ErrorType type)
    {
        ErrorCode = errorCode;
        Description = description;
        Type = type;
    }

    public static Error None = new Error(string.Empty, string.Empty, ErrorType.None);

    public static Error Validation(string description) => new Error("Error.Validation", description, ErrorType.Validation);

    public static Error Conflict(string description) => new Error("Error.Conflict", description, ErrorType.Conflict);

    public static Error NotFound(string description) => new Error("Error.NotFound", description, ErrorType.NotFound);
}