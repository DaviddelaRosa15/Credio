using Credio.Core.Domain.Contracts;

namespace Credio.Core.Domain.Events
{
    public class LoanEnteredArrearsEvent : IDomainEvent
    {
        public string LoanId { get; }
        public double Amount { get; }
        public double TotalAmount { get; }
        public int DaysInArrears { get; }

        public LoanEnteredArrearsEvent(string loanId, double amount, double totalAmount, int daysInArrears)
        {
            LoanId = loanId;
            Amount = amount;
            TotalAmount = totalAmount;
            DaysInArrears = daysInArrears;
        }
    }
}
