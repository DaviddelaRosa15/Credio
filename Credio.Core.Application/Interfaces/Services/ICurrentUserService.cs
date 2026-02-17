namespace Credio.Core.Application.Interfaces.Services;

public interface ICurrentUserService
{
    string? GetCurrentUserName();

    string? GetCurrentUserId();
}