using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class AmortizationStatusRepository : GenericRepository<AmortizationStatus>, IAmortizationStatusRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public AmortizationStatusRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
