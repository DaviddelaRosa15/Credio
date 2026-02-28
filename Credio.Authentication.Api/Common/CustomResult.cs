using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Response;
using Credio.Core.Application.Enums;

namespace Credio.Authentication.Api.Common;

public static class CustomResult
{
    public static IResult Success<TValue>(Result<TValue> result)
    {
        if (result.IsFailure)
        {
            throw new InvalidOperationException("Result indicates failure, but Success method was called.");
        }

        SuccessResponse<TValue> response = new SuccessResponse<TValue>
        {
            Data = result.Value
        };

        return Results.Ok(response);
    }

    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Result indicates success, but Problem method was called.");
        }

        (string title, string detail, string type, int statusCode) = GetErrorDetails(result.Error);

        return Results.Problem(
            title: title,
            detail: detail,
            type: type,
            statusCode: statusCode,
            extensions: GetErrorsFromResult(result));
    }

    private static (string title, string detail, string type, int statusCode) GetErrorDetails(Error error)
    {
        return error.Type switch
        {
            ErrorType.BadRequest => (error.ErrorCode, error.Description, "https://tools.ietf.org/html/rfc7231#section-6.5.1", StatusCodes.Status400BadRequest),
            ErrorType.NotFound => (error.ErrorCode, error.Description, "https://tools.ietf.org/html/rfc7231#section-6.5.4", StatusCodes.Status404NotFound),
            ErrorType.Conflict => (error.ErrorCode, error.Description, "https://tools.ietf.org/html/rfc7231#section-6.5.8", StatusCodes.Status409Conflict),
            ErrorType.InternalServerError => (error.ErrorCode, error.Description, "https://tools.ietf.org/html/rfc7231#section-6.6.1", StatusCodes.Status500InternalServerError),
            _ => ("Server Failure.", "An unexpected error occurred.", "https://tools.ietf.org/html/rfc7231#section-6.6.1", StatusCodes.Status500InternalServerError)
        };
    }
    private static Dictionary<string, object?>? GetErrorsFromResult(Result result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            { "errors", validationError.Errors }
        };
    }
}