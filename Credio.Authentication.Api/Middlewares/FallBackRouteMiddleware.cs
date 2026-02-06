using Microsoft.AspNetCore.Mvc;

namespace Credio.Authentication.Api.Middlewares;

public class FallBackRouteMiddleware : IMiddleware
{
    private readonly ILogger<FallBackRouteMiddleware> _logger;
    
    public FallBackRouteMiddleware(ILogger<FallBackRouteMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        if (context.Response is { StatusCode: StatusCodes.Status404NotFound, HasStarted: false, ContentLength: null })
        {
            _logger.LogInformation("Unhandled Route: {Method} {Path}", 
                context.Request.Method, 
                context.Request.Path);
            
            ProblemDetails details = new ProblemDetails
            {
                Title = "Not Found",
                Detail = "The requested resource was not found.",
                Status = StatusCodes.Status404NotFound,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4"
            };
            
            context.Response.ContentType = "application/problem+json";
            
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            
            await context.Response.WriteAsJsonAsync(details);
        }
    }
}