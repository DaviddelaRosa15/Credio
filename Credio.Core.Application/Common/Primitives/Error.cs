using System.Text.Json.Serialization;
using Credio.Core.Application.Enums;

namespace Credio.Core.Application.Common.Primitives;

public class Error
{
    public string ErrorCode { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    [JsonConstructor]
    protected Error(
        string errorCode,
        string description,
        ErrorType type)
    {
        ErrorCode = errorCode;
        Description = description;
        Type = type;
    }
    
    // Check the value not the reference
    public override bool Equals(object? obj)
    {
        if (obj is not Error other) return false;
        return ErrorCode == other.ErrorCode && Description == other.Description && Type == other.Type;
    }
    
    public override int GetHashCode() => HashCode.Combine(ErrorCode, Description, Type); // Need to be implemented

    public static bool operator ==(Error a, Error b) => a?.Equals(b) ?? b is null;

    public static bool operator !=(Error a, Error b) => !(a == b);

    public static Error None = new Error(string.Empty, string.Empty, ErrorType.None);

    public static Error Validation(string description) => new Error("Error.Validation", description, ErrorType.Validation);

    public static Error Conflict(string description) => new Error("Error.Conflict", description, ErrorType.Conflict);

    public static Error NotFound(string description) => new Error("Error.NotFound", description, ErrorType.NotFound);
}