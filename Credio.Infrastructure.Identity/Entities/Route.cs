using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class Route : AuditableBaseEntity
{
    public Route()
    {
        Clients = new HashSet<Client>();
    }
    
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } 

    public bool IsActive { get; set; }

    public string EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public ICollection<Client> Clients { get; set; }
}