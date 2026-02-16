using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<int> GetLastEmployeeCodeAsync();
    }
}
