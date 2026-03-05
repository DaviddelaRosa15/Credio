using System.ComponentModel.DataAnnotations.Schema;
using Credio.Core.Domain.Contracts;

namespace Credio.Core.Domain.Common;

public abstract class Entity
{
    public virtual string Id { get; set; }
    
    private readonly List<IDomainEvent> _domainEvents = [];
    
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void AddEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public void RemoveEvent(IDomainEvent @event)
    {
        _domainEvents.Remove(@event);
    }

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }

    public Entity()
    {
        Id = Guid.NewGuid().ToString().Substring(0, 12);
    }
}