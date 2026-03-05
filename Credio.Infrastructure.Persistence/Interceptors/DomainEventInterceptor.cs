using Credio.Core.Domain.Common;
using Credio.Core.Domain.Contracts;
using Credio.Infrastructure.Persistence.Channels;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Credio.Infrastructure.Persistence.Interceptors;

public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly List<IDomainEvent> _domainEvents = [];
    
    private readonly DomainEventChannel _domainEventChannel;

    public DomainEventInterceptor(DomainEventChannel domainEventChannel)
    {
        _domainEventChannel = domainEventChannel;
    }
    
    // Before save
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);
        
        IEnumerable<Entity> entities = eventData.Context.ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity);

        List<IDomainEvent> domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (Entity entity in entities) entity.ClearEvents();
            
        _domainEvents.AddRange(domainEvents);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    // After save
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (_domainEvents.Count <= 0) return await base.SavedChangesAsync(eventData, result, cancellationToken);

        foreach (IDomainEvent domainEvent in _domainEvents)
        {
            await _domainEventChannel.AddEventAsync(domainEvent, cancellationToken);
        }
        
        _domainEvents.Clear();

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}