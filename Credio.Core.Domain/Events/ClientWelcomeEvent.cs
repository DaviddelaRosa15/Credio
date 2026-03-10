using Credio.Core.Domain.Contracts;

namespace Credio.Core.Domain.Events;

public class ClientWelcomeEvent : IDomainEvent
{
    public string Username { get; }
    
    public ClientWelcomeEvent(string username)
    {
        Username = username;
    }
}