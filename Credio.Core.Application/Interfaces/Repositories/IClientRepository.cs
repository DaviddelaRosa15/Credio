using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.User;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<bool> IsDocumentNumberRegister(string documentNumber, CancellationToken cancellationToken);

        Task<PagedResult<ClientDto>> GetClientsPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            string? officerId,
            CancellationToken cancellationToken = default);
    }
}
