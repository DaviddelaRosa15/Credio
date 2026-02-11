using Credio.Core.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Credio.Lending.Api.ExceptionHandler;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError("An unhandled error happen with the error message : '{message}'", exception.Message);

        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "Server Failure.",
            Detail = "An unexpected error occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
        };

        Dictionary<string, object?>? errors = GetErrorsFromException(exception);

        if (errors is not null)
        {
            problemDetails.Extensions = errors;
        }

        httpContext.Response.StatusCode = (int)problemDetails.Status;

        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static Dictionary<string, object?>? GetErrorsFromException(Exception exception)
    {
        if (exception is not ValidationException validation)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            { "errors",validation.Failures }
        };
    }
}