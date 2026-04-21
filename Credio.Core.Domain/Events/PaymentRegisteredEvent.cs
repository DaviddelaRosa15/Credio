using Credio.Core.Domain.Contracts;

namespace Credio.Core.Domain.Events
{
    public class PaymentRegisteredEvent : IDomainEvent
    {
        public string PaymentId { get; }
        public decimal TotalInterestAppliedAmount { get; set; }
        public decimal TotalLateFeeAppliedAmount { get; set; }
        public decimal TotalPrincipalAppliedAmount { get; set; }

        public PaymentRegisteredEvent(string paymentId, decimal totalInterestAppliedAmount, decimal totalLateFeeAppliedAmount, decimal totalPrincipalAppliedAmount)
        {
            PaymentId = paymentId;
            TotalInterestAppliedAmount = totalInterestAppliedAmount;
            TotalLateFeeAppliedAmount = totalLateFeeAppliedAmount;
            TotalPrincipalAppliedAmount = totalPrincipalAppliedAmount;
        }
    }
}
