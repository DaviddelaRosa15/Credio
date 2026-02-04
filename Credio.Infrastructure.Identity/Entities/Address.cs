using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class Address : AuditableBaseEntity
{
    public Address()
    {
        Employees = new HashSet<Employee>();
        Clients = new HashSet<Client>();
    }
    
    public string? StreetNumber { get; set; }     

    public string AddressLine1 { get; set; } = string.Empty;
    
    public string? AddressLine2 { get; set; }
    
    public string City { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;  
    
    public string? PostalCode { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; }

    public ICollection<Client> Clients { get; set; }
}