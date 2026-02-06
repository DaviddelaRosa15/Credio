using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class LoanBalance : AuditableBaseEntity 
{
    public string LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public double TotalOutstanding { get; set; } 
    
    public double PrincipalBalance { get; set; } 
    
    public double InterestBalance { get; set; } 
    
    public double LateFeeBalance { get; set; }

    public int? DaysInArrears { get; set; }

    public DateTime? LastPaymentDate { get; set; }

    public DateTime? UpdatedAt { get; set; }
}