using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Common.Pipelines;

public class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(
        ICurrentUserService currentUserService,
        ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        string? username = _currentUserService.GetCurrentUserName();

        _logger.LogInformation(
            "Processing the request {requestName} requested by the user {user}",
            requestName,
            username ?? "System");

        TResponse response = await next(cancellationToken);

        if (response.IsSuccess)
        {
            _logger.LogInformation(
                "Completed the request {requestName} requested by the user {user}",
                requestName,
                username ?? "System");

            return response;
        }

        _logger.LogWarning(
            "Completed the request {requestName} requested by the user {user} with an error of type {errorName}",
            requestName,
            username ?? "System",
            response.Error.ErrorCode);

        return response;
    }
}