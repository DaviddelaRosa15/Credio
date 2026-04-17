using Credio.Core.Application.Dtos.Loan;
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

        public async Task<OfficerInfoDTO?> GetOfficerInfoByClientId(string clientId, CancellationToken cancellationToken)
        {
            using ApplicationContext context = _dbContext.CreateDbContext();

            return await context.Client
                .Include(x => x.Employee)
                .Where(x => x.Id == clientId)
                .Select(x => new OfficerInfoDTO
                {
                    OfficerFirstName = x.Employee.FirstName,
                    OfficerLastName = x.Employee.LastName,
                    OfficerEmail =  x.Employee.Email,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
