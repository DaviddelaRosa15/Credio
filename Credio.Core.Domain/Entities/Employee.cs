using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities
{
    public class Employee : AuditableBaseEntity
    {
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string? AddressId { get; set; }
        public List<Client> Clients { get; set; }
    }
}
