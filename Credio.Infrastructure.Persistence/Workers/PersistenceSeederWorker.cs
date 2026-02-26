using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Seeds;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Persistence.Workers;

public class PersistenceSeederWorker: BaseWorker<PersistenceSeederWorker>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PersistenceSeederWorker(
        ILogger<BaseWorker<PersistenceSeederWorker>> logger,
        IServiceScopeFactory scopeFactory) : base(logger)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        #region Document Types Seeding
        IDocumentTypeRepository documentTypeRepository = scope.ServiceProvider.GetRequiredService<IDocumentTypeRepository>();
        var anyDocumentType = await documentTypeRepository.GetAllAsync();
        if (anyDocumentType == null || anyDocumentType.Count == 0)
        {
            _logger.LogInformation("Seeding default document types...");
            await DefaultDocumentType.SeedAsync(documentTypeRepository);
        }
        else
        {
            _logger.LogInformation("Document types already exist. Skipping seeding.");
        }
        #endregion
        
        #region Application Status Seeding
        IApplicationStatusRepository applicationStatusRepository = scope.ServiceProvider.GetRequiredService<IApplicationStatusRepository>();

        List<ApplicationStatus> anyApplicationStatus = await applicationStatusRepository.GetAllAsync();
        
        if (anyApplicationStatus is null || anyApplicationStatus.Count == 0)
        {
            _logger.LogInformation("Seeding application status...");
            await DefaultApplicationStatus.Seed(applicationStatusRepository);
        }
        else
        {
            _logger.LogInformation("Application status already exists. Skipping seeding.");
        }

        #endregion
    }
}