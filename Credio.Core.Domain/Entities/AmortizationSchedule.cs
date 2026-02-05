using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class AmortizationSchedule : AuditableBaseEntity
{
    public AmortizationSchedule()
    {
        LateFees = [];
    }
    
    public string LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public int InstallmentNumber { get; set; }

    public DateTime DueDate { get; set; }

    public decimal DueAmount { get; set; } 

    public decimal InterestAmount { get; set; } 

    public decimal PrincipalAmount { get; set; } 

    public decimal RemainingBalance { get; set; } 

    public decimal? PaidAmount { get; set; } 

    public DateTime? LastPaymentDate { get; set; }

    public string AmortizationStatusId { get; set; }

    public AmortizationStatus AmortizationStatus { get; set; } = null!;

    public List<LateFee> LateFees { get; set; }
}