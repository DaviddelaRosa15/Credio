using System.Data;

namespace Credio.Core.Application.Interfaces.Persintence;

public interface IApplicationContext
{
    Task<IDbTransaction> GetDbTransactionAsync(CancellationToken cancellationToken = default);
}