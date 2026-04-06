namespace Credio.Core.Application.Interfaces.Services;

public interface ICurrentUserService
{
    string? GetCurrentUserName();

    string? GetCurrentUserId();
    
    bool isInRole(string role);
    
    List<string>? GetCurrentUserRoles();
}