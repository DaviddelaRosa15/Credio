using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class LoanBalanceRepository : GenericRepository<LoanBalance>, ILoanBalanceRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public LoanBalanceRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
