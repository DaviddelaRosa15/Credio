using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class Employee : AuditableBaseEntity
{
    public Employee()
    {
        Clients = new HashSet<Client>();
        Loans = new HashSet<Loan>();    
        LoanApplications = new HashSet<LoanApplication>();
        Routes = new HashSet<Route>();
        Payments = new HashSet<Payment>();
    }
    
    public string FirstName { get; set; } = string.Empty;   
    
    public string LastName { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;

    public string DocumentNumber { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;   
    
    public string? Email { get; set; } = string.Empty;

    public string EmployeeCode { get; set; } = string.Empty;

    public string ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public string AddressId { get; set; }

    public Address Address { get; set; } = null!;

    public ICollection<Client> Clients { get; set; }

    public ICollection<Loan> Loans { get; set; }

    public ICollection<LoanApplication> LoanApplications { get; set; }

    public ICollection<Route> Routes { get; set; }

    public ICollection<Payment> Payments { get; set; }
}