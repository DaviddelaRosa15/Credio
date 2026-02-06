using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities
{
    public class Employee : AuditableBaseEntity
    {
        public Employee()
        {
            Clients = [];
            Routes = [];
            Loans = [];
            LoanApplications = [];
            Payments = [];
        }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string DocumentTypeId { get; set; }
        
        public DocumentType DocumentType { get; set; } = null!;
        
        public string DocumentNumber { get; set; } 
        
        public string Phone { get; set; }
        
        public string? Email { get; set; }
        
        public string EmployeeCode { get; set; }
        
        public string UserId { get; set; }
        
        public string? AddressId { get; set; }

        public Address? Address { get; set; } 
        
        public List<Client> Clients { get; set; }
        
        public List<Route> Routes { get; set; }

        public List<Loan> Loans { get; set; }

        public List<LoanApplication> LoanApplications { get; set; }

        public List<Payment> Payments { get; set; }
    }
}
