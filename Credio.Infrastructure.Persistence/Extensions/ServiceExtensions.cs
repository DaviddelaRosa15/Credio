using Credio.Core.Application.Interfaces.Persintence;
using Credio.Core.Application.Interfaces.Services;
using Credio.Infrastructure.Persistence.Contexts;
using Credio.Infrastructure.Persistence.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Credio.Infrastructure.Persistence.Extensions;

public static partial class PersistenceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseInitializerService, DatabaseInitializerService>();

        services.AddScoped<IApplicationContext>(sp => sp.GetRequiredService<ApplicationContext>());
        
        return services;
    }
}