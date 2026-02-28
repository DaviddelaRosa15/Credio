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
        services.AddTransient<IClientRepository, ClientRepository>();
        services.AddTransient<IDocumentTypeRepository, DocumentTypeRepository>();
        services.AddTransient<IEmployeeRepository, EmployeeRepository>();
        services.AddTransient<IApplicationStatusRepository, ApplicationStatusRepository>();
        services.AddTransient<ILoanApplicationRepository, LoanApplicationRepository>();

        return services;
    }
}