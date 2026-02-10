using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Shared;


public abstract class BaseWorker<TWorker> : BackgroundService
    where TWorker : class
{
    protected readonly ILogger<BaseWorker<TWorker>> _logger;

    protected BaseWorker(ILogger<BaseWorker<TWorker>> logger)
    {
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Worker {workerName} start at {date} with {time}",
            typeof(TWorker).Name,
            DateTime.Now.ToShortDateString(),
            DateTime.Now.ToShortTimeString());

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        {
            _logger.LogInformation(
                "Worker {workerName} was signal to stop at {date} with {time}",
                typeof(TWorker).Name,
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToShortTimeString());
        });

        await RunAsync(stoppingToken);
    }

    public abstract Task RunAsync(CancellationToken cancellationToken);
}