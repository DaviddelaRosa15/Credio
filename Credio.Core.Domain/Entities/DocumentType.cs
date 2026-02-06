using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class DocumentType : BaseEntity
{
    public DocumentType()
    {
        Employees = [];
        Clients = [];
    }
    
    public List<Employee> Employees { get; set; }

    public List<Client> Clients { get; set; }
}