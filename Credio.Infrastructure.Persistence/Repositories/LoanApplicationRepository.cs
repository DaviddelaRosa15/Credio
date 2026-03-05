using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories;

public class LoanApplicationRepository : GenericRepository<LoanApplication>, ILoanApplicationRepository
{
    public LoanApplicationRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}