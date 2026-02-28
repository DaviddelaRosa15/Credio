using MediatR;

namespace Credio.Core.Domain.Contracts;

public interface IDomainEventHandler<in TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{ }