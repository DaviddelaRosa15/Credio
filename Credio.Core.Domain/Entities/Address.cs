using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities
{
    public class Address : AuditableBaseEntity
    {
        public string? StreetNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string? PostalCode { get; set; }

        public List<Client> Clients { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
