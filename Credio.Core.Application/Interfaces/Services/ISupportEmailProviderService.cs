namespace Credio.Core.Application.Interfaces.Services
{
    public interface ISupportEmailProviderService
    {
        Task<string> GetSupportEmailAsync();
    }
}