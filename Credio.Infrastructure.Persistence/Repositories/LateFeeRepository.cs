using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories;

public class LateFeeRepository : GenericRepository<LateFee>, ILateFeeRepository
{
    public LateFeeRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}