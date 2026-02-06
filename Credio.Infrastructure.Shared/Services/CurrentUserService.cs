using Credio.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Credio.Infrastructure.Shared.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;
    
    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext; 
    }
    
    public string? GetCurrentUserName()
    {
        // TODO: Replace with the corresponding claim.
        return _httpContext?.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == "Name")?.Value;
    }
}