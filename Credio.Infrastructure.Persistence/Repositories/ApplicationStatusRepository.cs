using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories;

public class ApplicationStatusRepository : GenericRepository<ApplicationStatus>, IApplicationStatusRepository
{
    public ApplicationStatusRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}