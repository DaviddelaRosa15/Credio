using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Common.Pipelines;

public class RequestPerformancePipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<RequestExceptionHandlingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestPerformancePipelineBehavior(ILogger<RequestExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            TResponse response = await next(cancellationToken);

            return response;
        }
        finally
        {
            stopwatch.Stop();

            string requestName = typeof(TRequest).Name;

            long elapsedTime = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation("The current request {requestName} completed in {elapsedTime}ms", requestName, elapsedTime);
        }
    }
}