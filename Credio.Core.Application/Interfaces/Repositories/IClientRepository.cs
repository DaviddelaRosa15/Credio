using Credio.Core.Application.Dtos.User;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<bool> IsDocumentNumberRegister(string documentNumber, CancellationToken cancellationToken);
        
        Task<ClientDto?> GetByDocumentNumberAsync(
            string documentNumber,
            CancellationToken cancellationToken);
    }
}
