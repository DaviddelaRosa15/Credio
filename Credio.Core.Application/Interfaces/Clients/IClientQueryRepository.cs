using Credio.Core.Application.Dtos.User;

namespace Credio.Core.Application.Interfaces.Clients 
{
    public interface IClientQueryRepository
    {
        Task<ClientListDto?> GetByDocumentNumberAsync(
            string documentNumber,
            CancellationToken cancellationToken);
    }
}


