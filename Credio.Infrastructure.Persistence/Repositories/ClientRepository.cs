using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public ClientRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsDocumentNumberRegister(string documentNumber,CancellationToken cancellationToken)
        {
            await using ApplicationContext context = await _dbContext.CreateDbContextAsync(cancellationToken);

            return await context.Client.AsNoTracking().AnyAsync(x => x.DocumentNumber == documentNumber, cancellationToken);
        }
    }
}
