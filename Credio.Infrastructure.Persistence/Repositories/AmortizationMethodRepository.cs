using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class AmortizationMethodRepository : GenericRepository<AmortizationMethod>, IAmortizationMethodRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public AmortizationMethodRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
