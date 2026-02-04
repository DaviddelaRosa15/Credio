using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class Client : AuditableBaseEntity
{
    public Client()
    {
        Loans = new HashSet<Loan>();    
        LoanApplications = new HashSet<LoanApplication>();  
    }
    
    public string FirstName { get; set; } = string.Empty;   
    
    public string LastName { get; set; } = string.Empty;

    public int? Age { get; set; }
    
    public string DocumentType { get; set; } = string.Empty;

    public string DocumentNumber { get; set; } = string.Empty;
    
    public string Phone { get; set; } = string.Empty;   
    
    public string? Email { get; set; } = string.Empty;

    public string AddressId { get; set; }

    public Address Address { get; set; } = null!;

    public string ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public string EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public string? RouteId { get; set; }

    public Route? Route { get; set; }

    public double? HomeLatitude { get; set; } 
    
    public double? HomeLongitude { get; set; } 
    
    public ICollection<Loan> Loans { get; set; }

    public ICollection<LoanApplication> LoanApplications { get; set; }
}
