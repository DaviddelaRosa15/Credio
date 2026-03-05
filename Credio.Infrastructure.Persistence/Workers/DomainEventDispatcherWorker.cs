using Credio.Core.Application.Common;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Contracts;
using Credio.Infrastructure.Persistence.Channels;
using Credio.Infrastructure.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Persistence.Workers;

public class DomainEventDispatcherWorker : BaseWorker<DomainEventDispatcherWorker>
{
    private readonly DomainEventChannel _domainEventChannel;
    private readonly IPublisher _publisher;
    private readonly IExponentialBackoffService _exponentialBackoffService;

    public DomainEventDispatcherWorker(
        ILogger<BaseWorker<DomainEventDispatcherWorker>> logger,
        DomainEventChannel domainEventChannel,
        IPublisher publisher,
        IExponentialBackoffService exponentialBackoffService) : base(logger)
    {
        _domainEventChannel = domainEventChannel;
        _publisher = publisher;
        _exponentialBackoffService = exponentialBackoffService;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        await foreach (IDomainEvent @event in _domainEventChannel.ReadAllAsync(cancellationToken))
        {
            try
            {
                await _publisher.Publish(@event, cancellationToken);

                _logger.LogInformation("Successfully publish the event {eventName}", @event.GetType().Name);

            } catch (Exception ex)
            {
                _logger.LogError(
                    "An unexpected error happen while trying to publish the domain event {eventName} with the error message : {message}, retrying...",
                    @event.GetType().Name,
                    ex.Message);

                await _exponentialBackoffService.RetryWithBackoff(
                    () => _publisher.Publish(@event, cancellationToken),                                                           
                    new BackOffOptions(3, 100, 5000, 2),
                    cancellationToken);
            }
        }
    }
}