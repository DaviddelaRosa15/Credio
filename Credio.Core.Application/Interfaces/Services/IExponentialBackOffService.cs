using Credio.Core.Application.Common;

namespace Credio.Core.Application.Interfaces.Services;

public interface IExponentialBackoffService
{
    Task RetryWithBackoff(Func<Task> func, BackOffOptions? options = null, CancellationToken cancellationToken = default);

    Task<T?> RetryWithBackoff<T >(Func<Task<T>> func, BackOffOptions? options = null, CancellationToken cancellationToken = default);
}