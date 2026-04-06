using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories;

public class LateFeeStatusRepository : GenericRepository<LateFeeStatus>, ILateFeeStatusRepository
{
    public LateFeeStatusRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}