using Credio.Core.Domain.Contracts;

namespace Credio.Core.Domain.Events
{
    public class LoanCreatedAmortizationEvent : IDomainEvent
    {
        public double Amount { get; }
        public int DaysInterval { get; }
        public DateOnly FirstPaymentDate { get; }
        public double InterestRate { get; }
        public string LoanId { get; }
        public int Term { get; }

        public LoanCreatedAmortizationEvent(string loanId, double amount, int term, double interestRate,
            DateOnly firstPaymentDate, int daysInterval)
        {
            Amount = amount;
            DaysInterval = daysInterval;
            FirstPaymentDate = firstPaymentDate;
            InterestRate = interestRate;
            LoanId = loanId;
            Term = term;
        }
    }
}
