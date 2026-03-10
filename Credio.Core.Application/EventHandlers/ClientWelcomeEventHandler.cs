using Credio.Core.Domain.Contracts;
using Credio.Core.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.EventHandlers;

public class ClientWelcomeEventHandler : IDomainEventHandler<ClientWelcomeEvent>
{
    private readonly ILogger<ClientWelcomeEventHandler> _logger;

    public ClientWelcomeEventHandler(ILogger<ClientWelcomeEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(ClientWelcomeEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Welcome to Credio, username: {username}", notification.Username);
        
        return Task.CompletedTask;
    }
}