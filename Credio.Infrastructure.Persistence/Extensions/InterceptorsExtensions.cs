using Credio.Infrastructure.Persistence.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Credio.Infrastructure.Persistence.Extensions;

public static partial class InfrastructureExtensions
{
    public static IServiceCollection AddInterceptors(this IServiceCollection services)
    {
        services.AddSingleton<AuditableEntityInterceptor>();
        services.AddSingleton<SoftDeleteInterceptor>();

        return services;
    }
}