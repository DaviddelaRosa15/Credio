using Credio.Core.Application.Interfaces.Services;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Persistence.Services;

public sealed class DatabaseInitializerService : IDatabaseInitializerService
{
    private readonly IDbContextFactory<ApplicationContext> _dbContext;
    private readonly ILogger<DatabaseInitializerService> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public DatabaseInitializerService(
        IDbContextFactory<ApplicationContext> dbContext,
        ILogger<DatabaseInitializerService> logger,
        IHostEnvironment  hostEnvironment)
    {
        _dbContext = dbContext;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task CanConnectAsync(CancellationToken cancellationToken = default)
    {
        await using DbContext context = await _dbContext.CreateDbContextAsync(cancellationToken);
        
        bool isConnected = await context.Database.CanConnectAsync(cancellationToken);

        if (!isConnected)
        {
            _logger.LogWarning("Cant connect to the database, database not available");

            throw new InvalidOperationException();
        }

        _logger.LogInformation("Successfully connect to the database!!!.");
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        if (_hostEnvironment.IsDevelopment())
        {
            _logger.LogInformation("Skipping migrations in Development environment.");
            return;
        }
        
        await using DbContext context = await _dbContext.CreateDbContextAsync(cancellationToken);
        
        try
        {
            await context.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Successfully apply the migrations!!.");

        }
        catch (Exception ex)
        {
            _logger.LogError(
                "An error happen trying to apply migrations to the database {providerName} with the error message: {message}",
                context.Database.ProviderName,
                ex.Message);

            throw;
        }
    }
}