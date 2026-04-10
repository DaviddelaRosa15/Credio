using Credio.Core.Domain.Contracts;

namespace Credio.Core.Domain.Events
{
    public class LoanEnteredArrearsEvent : IDomainEvent
    {
        public string LoanId { get; }
        public double Amount { get; }
        public int DaysInArrears { get; }

        public LoanEnteredArrearsEvent(string loanId, double amount, int daysInArrears)
        {
            LoanId = loanId;
            Amount = amount;
            DaysInArrears = daysInArrears;
        }
    }
}
