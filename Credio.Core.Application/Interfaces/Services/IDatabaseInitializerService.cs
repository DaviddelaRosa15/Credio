namespace Credio.Core.Application.Interfaces.Services;

public interface IDatabaseInitializerService
{
    Task MigrateAsync(CancellationToken cancellationToken = default);

    Task CanConnectAsync(CancellationToken cancellationToken = default);
}