using Credio.Infrastructure.Persistence.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Credio.Infrastructure.Persistence.Extensions;

public static partial class PersistenceExtensions
{
    public static IServiceCollection AddWorkers(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseInitializerWorker>();

        return services;
    }
}