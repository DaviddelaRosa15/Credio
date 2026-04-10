using Credio.Core.Application.Dtos.CoreConfiguration;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IEndOfDayService
    {
        Task<string> PrepareAsync();
        Task<EndOfDayProcessResponseDTO> ProcessQueueAsync(string logId);
    }
}
