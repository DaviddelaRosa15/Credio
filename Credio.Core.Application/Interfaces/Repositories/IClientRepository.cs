using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<bool> IsDocumentNumberRegister(string documentNumber, CancellationToken cancellationToken);

        Task<OfficerInfoDTO?> GetOfficerInfoByClientId(string clientId, CancellationToken cancellationToken);
    }
}