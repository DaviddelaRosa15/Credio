using Credio.Core.Domain.Contracts;

namespace Credio.Core.Domain.Events
{
    public class LoanDisbursedEvent : IDomainEvent
    {
        public string LoanId { get; }
        public DateOnly DisbursedDate { get; }

        public LoanDisbursedEvent(string loanId, DateOnly disbursedDate)
        {
            LoanId = loanId;
            DisbursedDate = disbursedDate;
        }
    }
}
