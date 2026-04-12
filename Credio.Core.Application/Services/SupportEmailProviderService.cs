using Credio.Core.Application.Constants;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Credio.Core.Application.Services
{
    public class SupportEmailProviderService : ISupportEmailProviderService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _fallbackEmail;

        public SupportEmailProviderService(IServiceProvider serviceProvider, IOptions<TechnicalAlertSettings> options)
        {
            _serviceProvider = serviceProvider;
            _fallbackEmail = options.Value.DefaultSupportEmail; // El paracaídas
        }

        public async Task<string> GetSupportEmailAsync()
        {
            try
            {
                // Creamos un scope temporal solo para consultar el setting
                using var scope = _serviceProvider.CreateScope();
                var settingsRepo = scope.ServiceProvider.GetRequiredService<ISystemSettingsRepository>();

                var setting = await settingsRepo.GetByPropertyAsync(s => s.Key == EndOfDaySettings.TechSupportEmailKey);
                return setting?.Value ?? _fallbackEmail;
            }
            catch
            {
                // Si la DB no responde o el repo falla, devolvemos el fallback sin lanzar excepción
                return _fallbackEmail;
            }
        }
    }
}
