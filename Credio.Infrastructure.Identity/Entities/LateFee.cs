using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class LateFee : AuditableBaseEntity
{
    public string LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public string AmortizationScheduleId { get; set; }

    public AmortizationSchedule AmortizationSchedule { get; set; } = null!;

    public double Amount { get; set; } 

    public double Balance { get; set; } 

    public string LateFeeStatusId { get; set; }

    public LateFeeStatus LateFeeStatus { get; set; } = null!;

    public DateTime? GeneratedDate { get; set; }

    public DateTime? PaidDate { get; set; }
}