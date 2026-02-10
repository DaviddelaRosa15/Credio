using Credio.Authentication.Api.Middlewares;

namespace Credio.Interface.Authentication.Extensions;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<FallBackRouteMiddleware>();
        
        return services;
    }
}