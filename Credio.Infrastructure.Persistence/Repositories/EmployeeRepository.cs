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

            // 1. Ejecutamos el filtro básico en SQL y traemos los datos a memoria (ToListAsync)
            var employeeCodes = await db.Employee
                .Where(e => e.EmployeeCode != null && e.EmployeeCode.StartsWith("U"))
                .Select(e => e.EmployeeCode)
                .ToListAsync();

            // 2. Ahora que los datos están en memoria (C#), hacemos la extracción y el Max()
            var lastCode = employeeCodes
                .Select(code => int.Parse(code.Substring(1)))
                .DefaultIfEmpty(0)
                .Max();

            return lastCode;
        }
    }
}
