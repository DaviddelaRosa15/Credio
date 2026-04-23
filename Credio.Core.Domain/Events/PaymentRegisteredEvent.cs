using Credio.Core.Domain.Contracts;

namespace Credio.Core.Domain.Events
{
    public class PaymentRegisteredEvent : IDomainEvent
    {
        public string PaymentId { get; }

        public PaymentRegisteredEvent(string paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
