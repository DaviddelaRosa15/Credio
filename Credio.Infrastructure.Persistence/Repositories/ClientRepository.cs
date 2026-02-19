using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Credio.Core.Application.Dtos.User;
using Credio.Core.Application.Dtos.Common;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public ClientRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsDocumentNumberRegister(string documentNumber, CancellationToken cancellationToken)
        {
            await using ApplicationContext context = await _dbContext.CreateDbContextAsync(cancellationToken);

            return await context.Client.AsNoTracking().AnyAsync(x => x.DocumentNumber == documentNumber, cancellationToken);
        }

        public async Task<PagedResult<ClientDto>> GetClientsPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            string? officerId,
            CancellationToken cancellationToken = default)
        {
            using var context = _dbContext.CreateDbContext();

            IQueryable<Client> query = context.Client
                .Include(c => c.Address)
                .AsNoTracking();

           
            if (!string.IsNullOrWhiteSpace(officerId))
            {
                query = query.Where(c => c.EmployeeId == officerId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();

                query = query.Where(c =>
                    c.FirstName.ToLower().Contains(term) ||
                    c.LastName.ToLower().Contains(term) ||
                    c.DocumentNumber.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync(cancellationToken);

            List<Client> clients = await query
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            List<ClientDto> items = clients.Select(c => new ClientDto
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                DocumentNumber = c.DocumentNumber,
                Phone = c.Phone,
                City = c.Address?.City ?? string.Empty,
                State = c.Address?.Region ?? string.Empty,
            }).ToList();

            return new PagedResult<ClientDto>(items, totalCount, pageNumber, pageSize);
        }

    }
}
