using Credio.Core.Application.Dtos.User;
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

        public async Task<ClientDto?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken)
        {
            await using ApplicationContext context = await _dbContext.CreateDbContextAsync(cancellationToken);
            
            return await context.Client
                .AsNoTracking()
                .Where(c => c.DocumentNumber == documentNumber)
                .Select(c => new ClientDto()
                {
                    Id = c.Id,
                    FullName = c.FirstName + " " + c.LastName,
                    DocumentNumber = c.DocumentNumber,
                    Phone = c.Phone,
                    City = c.Address.City,
                    State = c.Address.Region
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
