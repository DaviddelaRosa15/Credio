using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class Route : AuditableBaseEntity
{
    public Route()
    {
        Clients = [];
    }
    
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } 

    public bool IsActive { get; set; }

    public string EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public List<Client> Clients { get; set; }
}