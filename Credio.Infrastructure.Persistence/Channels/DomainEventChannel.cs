using System.Threading.Channels;
using Credio.Core.Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Persistence.Channels;

public class DomainEventChannel
{
    private readonly ILogger<DomainEventChannel> _logger;
    private readonly Channel<IDomainEvent> _channel;
    
    private const int MaxEvents = 1_00; // 100 

    public DomainEventChannel(ILogger<DomainEventChannel> logger)
    {
        BoundedChannelOptions options = new BoundedChannelOptions(MaxEvents)
        {
            SingleWriter = false,
            SingleReader = true,
        };

        _channel = Channel.CreateBounded<IDomainEvent>(options);
        _logger = logger;
    }
    
    public async Task<bool> AddEventAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            if (_channel.Writer.TryWrite(@event))
            {
                _logger.LogInformation("The domain event {event} was enqueue into the channel", @event.GetType().Name);

                return true;
            }
        }

        return false;
    }

    public IAsyncEnumerable<IDomainEvent> ReadAllAsync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}