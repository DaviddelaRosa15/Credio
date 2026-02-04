using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class LoanApplication : AuditableBaseEntity
{
    public LoanApplication()
    {
        Loans = new HashSet<Loan>();
    }
    
    public string ApplicationCode { get; set; } = string.Empty;

    public string ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public string EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public double RequestedAmount { get; set; } 

    public int RequestTerm { get; set; }

    public double RequestedInterestRate { get; set; } 

    public string? Purpose { get; set; } = string.Empty;

    public double? ApprovedAmount { get; set; }  

    public int? ApprovedTerm { get; set; }

    public double? ApprovedInterestRate { get; set; } 

    public string ApplicationStatusId { get; set; }

    public ApplicationStatus ApplicationStatus { get; set; } = null!;

    public string? RejectionReason { get; set; } = string.Empty;

    public ICollection<Loan> Loans { get; set; }
}