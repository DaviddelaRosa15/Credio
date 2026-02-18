using Credio.Core.Application.Dtos.User;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Credio.Core.Application.Interfaces.Clients;

namespace Credio.Infrastructure.Persistence.Repositories 
{
    public class ClientQueryRepository : IClientQueryRepository
    {
        private readonly ApplicationContext _context;

        public ClientQueryRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ClientListDto?> GetByDocumentNumberAsync(
            string documentNumber,
            CancellationToken cancellationToken)
        {
            return await _context.Client
                .AsNoTracking()
                .Where(c => c.DocumentNumber == documentNumber)
                .Select(c => new ClientListDto
                {
                    Id = c.Id,
                    FullName = c.FirstName + " " + c.LastName,
                    DocumentNumber = c.DocumentNumber,
                    Phone = c.Phone,
                    City = c.Address.City,
                    State = c.Address.Region
                })
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}


