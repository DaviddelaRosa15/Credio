using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public EmployeeRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetLastEmployeeCodeAsync()
        {
            using var db = _dbContext.CreateDbContext();

            // Obtener el último código de empleado registrado
            var lastEmployee = db.Employee.Max(e => e.EmployeeCode);
            int lastCode = string.IsNullOrEmpty(lastEmployee) ? 0 : int.Parse(lastEmployee.Replace("U", ""));

            return lastCode;
        }
    }
}
