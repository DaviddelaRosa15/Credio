using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class EndOfDayQueueRepository : GenericRepository<EndOfDayQueue>, IEndOfDayQueueRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public EndOfDayQueueRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}