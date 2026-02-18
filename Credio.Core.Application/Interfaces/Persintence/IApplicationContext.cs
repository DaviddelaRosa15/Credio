using Microsoft.EntityFrameworkCore.Storage;

namespace Credio.Core.Application.Interfaces.Persintence;

public interface IApplicationContext
{
    Task<IDbContextTransaction> GetDbTransactionAsync(CancellationToken cancellationToken = default);
}