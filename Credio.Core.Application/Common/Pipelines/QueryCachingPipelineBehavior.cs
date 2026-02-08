using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Common.Pipelines;

public class QueryCachingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : Result
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<QueryCachingPipelineBehavior<TRequest, TResponse>> _logger;

    public QueryCachingPipelineBehavior(
        ICacheService cacheService,
        ILogger<QueryCachingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse? cachedResult = _cacheService.Get<TResponse>(request.CachedKey);

        string requestName = typeof(TRequest).Name;

        if (cachedResult is not null)
        {
            _logger.LogInformation("Cache hit for the request {requestName}", requestName);

            return cachedResult;
        }

        _logger.LogInformation("Cache miss for the request {requestName}", requestName);

        TResponse response = await next();

        if (response.IsSuccess)
        {
            _cacheService.Set(request.CachedKey, response,request.Options);
        }

        return response;
    }
}