using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class AmortizationScheduleRepository : GenericRepository<AmortizationSchedule>, IAmortizationScheduleRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public AmortizationScheduleRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
