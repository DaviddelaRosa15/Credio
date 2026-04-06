using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities
{
    public class EndOfDayQueue : AuditableBaseEntity
    {
        public string LogId { get; set; } = null!;
        public EndOfDayExecutionLog Log { get; set; } = null!;

        public string LoanId { get; set; } = null!;
        public Loan Loan { get; set; } = null!;

        public string Status { get; set; } = null!; // "Pending", "Processed", "Failed"
        public string? ErrorMessage { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}