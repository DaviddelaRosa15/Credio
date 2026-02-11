using Credio.Lending.Api.Middlewares;

namespace Credio.Interface.Lending.Extensions;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<FallBackRouteMiddleware>();
        
        return services;
    }
}