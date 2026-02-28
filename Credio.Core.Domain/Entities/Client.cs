using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities
{
    public class Client : AuditableBaseEntity
    {
        public Client()
        {
            Loans = [];
            LoanApplications = [];
        }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public int Age { get; set; }
        
        public string DocumentTypeId { get; set; }

        public DocumentType DocumentType { get; set; } = null!;
        
        public string DocumentNumber { get; set; }
        
        public string Phone { get; set; }
        
        public string? Email { get; set; }
        
        public string AddressId { get; set; }
        
        public Address Address { get; set; } = null!;
        
        public string UserId { get; set; }

        public string EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;
        
        public string? RouteId { get; set; }
        
        public Route? Route { get; set; }
        
        public decimal HomeLatitude { get; set; }
        
        public decimal HomeLongitude { get; set; }
        
        public List<Loan> Loans { get; set; }

        public List<LoanApplication> LoanApplications { get; set; }
    }
}
