using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Credio.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Credio.Infrastructure.Identity.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;
    
    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext; 
    }
    
    public string? GetCurrentUserName()
    {
        return _httpContext?.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    public string? GetCurrentUserId()
    {
        return _httpContext.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value;
    }
}