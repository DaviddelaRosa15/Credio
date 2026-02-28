using Credio.Core.Application.Common;
using Credio.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Helpers;

public class ExponentialBakOffService : IExponentialBackoffService
{
    private readonly ILogger<ExponentialBakOffService> _logger;
    
    private readonly Random _random = new Random();

    public ExponentialBakOffService(ILogger<ExponentialBakOffService> logger)
    {
        _logger = logger;
    }
    
    public async Task RetryWithBackoff(Func<Task> func, BackOffOptions? options = null, CancellationToken cancellationToken = default)
    {
        (int maxRetries, int initialDelay, int maxDelay, int timeMultiple) = options ?? new BackOffOptions();

        int attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                await func();

                _logger.LogInformation(
                    "Operation succeeded after retry count {attempts}/{maxRetries} retries.",
                    attempts,
                    maxRetries);

                return;
            }
            catch
            {
                int delay = CalculateDelay(options ?? new BackOffOptions(maxRetries, initialDelay, maxDelay, timeMultiple), attempts);

                await Task.Delay(delay, cancellationToken);

                attempts++;

                _logger.LogError(
                    "Operation failed after retry count {attempts}/{maxRetries} and {delay}ms.",
                    attempts,
                    maxRetries,
                    delay);
            }
        }

        _logger.LogError("Max retries reached. Operation ultimately failed.");
    }

    public async Task<T?> RetryWithBackoff<T>(Func<Task<T>> func, BackOffOptions? options = null, CancellationToken cancellationToken = default)
    {
        (int maxRetries, int initialDelay, int maxDelay, int timeMultiple) = options ?? new BackOffOptions();

        int attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                T result = await func();

                _logger.LogInformation(
                    "Operation succeeded after retry count {attempts}/{maxRetries} retries.",
                    attempts,
                    maxRetries);

                return result;

            } catch
            {
                int delay = CalculateDelay(options ?? new BackOffOptions(maxRetries, initialDelay, maxDelay, timeMultiple), attempts);

                await Task.Delay(delay, cancellationToken);

                attempts++;

                _logger.LogError(
                    "Operation failed after retry count {attempts}/{maxRetries} and {delay}ms.",                    
                    attempts,
                    maxRetries,
                    delay);
            }
        }

        _logger.LogError("Max retries reached. Operation ultimately failed.");

        return default(T);
    }
    
    private int CalculateDelay(BackOffOptions options, int attempts)
    {
        (int maxRetries, int initialDelay, int maxDelay, int timeMultiple) = options;

        double waitTime = Math.Min(initialDelay * Math.Pow(timeMultiple, attempts), maxDelay);

        return (int)Math.Floor(_random.NextDouble() * waitTime);
    }
}