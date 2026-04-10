using Credio.Core.Application.Interfaces.Repositories;
using Credio.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Credio.Infrastructure.Persistence.Extensions;

public static partial class PersistenceExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IAddressRepository, AddressRepository>();
        services.AddTransient<IAmortizationMethodRepository, AmortizationMethodRepository>();
        services.AddTransient<IAmortizationScheduleRepository, AmortizationScheduleRepository>();
        services.AddTransient<IAmortizationStatusRepository, AmortizationStatusRepository>();
        services.AddTransient<IClientRepository, ClientRepository>();
        services.AddTransient<IDocumentTypeRepository, DocumentTypeRepository>();
        services.AddTransient<IEmployeeRepository, EmployeeRepository>();
        services.AddTransient<IEndOfDayExecutionLogRepository, EndOfDayExecutionLogRepository>();
        services.AddTransient<IEndOfDayQueueRepository, EndOfDayQueueRepository>();
        services.AddTransient<IApplicationStatusRepository, ApplicationStatusRepository>();
        services.AddTransient<ILateFeeRepository, LateFeeRepository>();
        services.AddTransient<ILateFeeStatusRepository, LateFeeStatusRepository>();
        services.AddTransient<ILoanApplicationRepository, LoanApplicationRepository>();
        services.AddTransient<ILoanBalanceRepository, LoanBalanceRepository>();
        services.AddTransient<ILoanStatusRepository, LoanStatusRepository>();
        services.AddTransient<ILoanRepository, LoanRepository>();
        services.AddTransient<IPaymentFrequencyRepository, PaymentFrequencyRepository>();
        services.AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddTransient<IPaymentStatusRepository, PaymentStatusRepository>();
        services.AddTransient<ISystemSettingsRepository, SystemSettingsRepository>();

        return services;
    }
}