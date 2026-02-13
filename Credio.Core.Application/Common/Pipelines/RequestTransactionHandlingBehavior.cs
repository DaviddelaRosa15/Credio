using System.Data;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Persintence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Common.Pipelines;

public class RequestTransactionHandlingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    private readonly ILogger<RequestTransactionHandlingBehavior<TRequest, TResponse>> _logger;
    private readonly IApplicationContext _applicationContext;
    
    public RequestTransactionHandlingBehavior(
        ILogger<RequestTransactionHandlingBehavior<TRequest, TResponse>> logger, 
        IApplicationContext applicationContext)
    {
        _logger = logger;
        _applicationContext = applicationContext;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using IDbTransaction transaction = await _applicationContext.GetDbTransactionAsync(cancellationToken);
        
        string commandName = typeof(TRequest).Name;
        
        try
        {
            TResponse response = await next(cancellationToken);
            
            _logger.LogInformation(
                "The command {commandName} was completed succesfully, committing the transaction.",
                commandName);

            transaction.Commit();

            return response;
        }
        catch
        {
            _logger.LogError(
                "An unexpected error happen while trying to complete the command {commandName}, rollying back the transaction.",
                commandName);
            
            transaction.Rollback();
            
            throw;
        }
    }
}