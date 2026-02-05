using Credio.Core.Application.Interfaces.Repositories;
using Credio.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Credio.Infrastructure.Persistence.Extensions;

public static partial class InfrastructureExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IClientRepository, ClientRepository>();
        services.AddTransient<IEmployeeRepository, EmployeeRepository>();

        return services;
    }
}