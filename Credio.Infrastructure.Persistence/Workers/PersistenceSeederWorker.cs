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

        #region Loan Status Seeding
        ILoanStatusRepository loanStatusRepository = scope.ServiceProvider.GetRequiredService<ILoanStatusRepository>();

        List<LoanStatus> anyLoanStatus = await loanStatusRepository.GetAllAsync();

        if (anyLoanStatus is null || anyLoanStatus.Count == 0)
        {
            _logger.LogInformation("Seeding loan status...");
            await DefaultLoanStatus.SeedAsync(loanStatusRepository);
        }
        else
        {
            _logger.LogInformation("Loan status already exists. Skipping seeding.");
        }
        #endregion

        #region Amortization Status Seeding
        IAmortizationStatusRepository amortizationStatusRepository = scope.ServiceProvider.GetRequiredService<IAmortizationStatusRepository>();

        List<AmortizationStatus> anyAmortizationStatus = await amortizationStatusRepository.GetAllAsync();

        if (anyAmortizationStatus is null || anyAmortizationStatus.Count == 0)
        {
            _logger.LogInformation("Seeding amortization status...");
            await DefaultAmortizationStatus.SeedAsync(amortizationStatusRepository);
        }
        else
        {
            _logger.LogInformation("Amortization status already exists. Skipping seeding.");
        }
        #endregion

        #region Payment Frequency Seeding
        IPaymentFrequencyRepository paymentFrequencyRepository = scope.ServiceProvider.GetRequiredService<IPaymentFrequencyRepository>();

        List<PaymentFrequency> anyPaymentFrequency = await paymentFrequencyRepository.GetAllAsync();

        if (anyPaymentFrequency is null || anyPaymentFrequency.Count == 0)
        {
            _logger.LogInformation("Seeding payment frequency...");
            await DefaultPaymentFrequency.SeedAsync(paymentFrequencyRepository);
        }
        else
        {
            _logger.LogInformation("Payment Frequency already exists. Skipping seeding.");
        }
        #endregion

        #region Amortization Method Seeding
        IAmortizationMethodRepository amortizationMethodRepository = scope.ServiceProvider.GetRequiredService<IAmortizationMethodRepository>();

        List<AmortizationMethod> anyAmortizationMethod = await amortizationMethodRepository.GetAllAsync();

        if (anyAmortizationMethod is null || anyAmortizationMethod.Count == 0)
        {
            _logger.LogInformation("Seeding amortization method...");
            await DefaultAmortizationMethod.SeedAsync(amortizationMethodRepository);
        }
        else
        {
            _logger.LogInformation("Amortization Method already exists. Skipping seeding.");
        }
        #endregion

        #region System settings Seeding
        ISystemSettingsRepository systemSettingsRepository = scope.ServiceProvider.GetRequiredService<ISystemSettingsRepository>();

        List<SystemSettings> anySystemSettings = await systemSettingsRepository.GetAllAsync();

        if (anySystemSettings is null || anySystemSettings.Count == 0)
        {
            _logger.LogInformation("Seeding system settings...");
            await DefaultSystemSettings.SeedAsync(systemSettingsRepository);
        }
        else
        {
            _logger.LogInformation("System settings already exists. Skipping seeding.");
        }
        #endregion

        #region Late Fee Status Seeding
        ILateFeeStatusRepository lateFeeStatusRepository = scope.ServiceProvider.GetRequiredService<ILateFeeStatusRepository>();

        List<LateFeeStatus> anylateFeeStatuses = await lateFeeStatusRepository.GetAllAsync();

        if (anylateFeeStatuses is null || anylateFeeStatuses.Count == 0)
        {
            _logger.LogInformation("Seeding late fee statuses...");
            await DefaultLateFeeStatus.SeedAsync(lateFeeStatusRepository);
        }
        else
        {
            _logger.LogInformation("Late fee statuses already exists. Skipping seeding.");
        }
        #endregion
    }
}