using Credio.Core.Application.Dtos.CoreConfiguration;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IEodAlertService
    {
        Task SendEodAlertAsync(string message, string? exception);
    }
}