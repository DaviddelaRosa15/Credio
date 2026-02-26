using Credio.Core.Application.Interfaces.Services;
using Credio.Infrastructure.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Persistence.Workers;

public class DatabaseInitializerWorker : BaseWorker<DatabaseInitializerWorker>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseInitializerWorker(
        ILogger<BaseWorker<DatabaseInitializerWorker>> logger,
        IServiceScopeFactory scopeFactory) : base(logger)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        
        IDatabaseInitializerService initializerService = scope.ServiceProvider.GetRequiredService<IDatabaseInitializerService>();

        await initializerService.CanConnectAsync(cancellationToken);

        await initializerService.MigrateAsync(cancellationToken);
    }
}