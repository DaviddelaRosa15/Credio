using Credio.Core.Application.Constants;
using Credio.Core.Application.Features.CoreConfiguration.Commands.PrepareEndOfDay;
using Credio.Core.Application.Features.CoreConfiguration.Commands.ProcessEndOfDayQueue;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Settings;
using Credio.Infrastructure.Shared;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credio.Infrastructure.Persistence.Workers
{
    public class EndOfDayProcessingWorker : BaseWorker<EndOfDayProcessingWorker>
    {
        private readonly EndOfDayLogSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private DateOnly? _lastExecutionDate; // Cache de la última ejecución exitosa

        public EndOfDayProcessingWorker(
            ILogger<EndOfDayProcessingWorker> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<EndOfDayLogSettings> settings)
            : base(logger)
        {
            _scopeFactory = scopeFactory;
            _settings = settings.Value;
        }

        public override async Task RunAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando ciclo de verificación para proceso automático de COB.");
                    // Todo el scope debe crearse DENTRO del bucle para tener datos frescos en cada vuelta
                    using IServiceScope scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var settingsRepo = scope.ServiceProvider.GetRequiredService<ISystemSettingsRepository>();
                    var endOfDayLogRepo = scope.ServiceProvider.GetRequiredService<IEndOfDayExecutionLogRepository>();

                    DateTime now = DateTime.Now;
                    DateOnly today = DateOnly.FromDateTime(now);

                    var existingLog = await endOfDayLogRepo.GetByPropertyAsync(log => log.ExecutionDate == today);
                    if (existingLog is not null && (existingLog.Status == EndOfDayLogStatuses.Completed || existingLog.Status == EndOfDayLogStatuses.CompletedWithErrors))
                    {
                        _lastExecutionDate = today; // Actualizamos el cache para evitar reintentos innecesarios
                        _logger.LogInformation("El proceso de COB ya se ejecutó hoy con estado: {Status}. No se permitirá otra ejecución hasta mañana.", existingLog.Status);
                        await Task.Delay(TimeSpan.FromMinutes(_settings.CheckFrequencyInMinutes), stoppingToken);
                        continue; // Saltamos a la siguiente iteración sin hacer nada
                    }

                    // 1. Obtener la hora programada
                    var cobTimeSetting = await settingsRepo.GetByPropertyAsync(s => s.Key == EndOfDaySettings.COBExecutionTimeKey);

                    if (cobTimeSetting != null && TimeOnly.TryParse(cobTimeSetting.Value, out TimeOnly scheduledTime))
                    {
                        // 2. Lógica de Disparo
                        if (now.TimeOfDay >= scheduledTime.ToTimeSpan() && _lastExecutionDate != today)
                        {
                            _logger.LogInformation("Iniciando proceso automático de COB programado para las {Time}", scheduledTime);

                            var prepareResult = await mediator.Send(new PrepareEndOfDayCommand(), stoppingToken);

                            if (prepareResult.IsSuccess)
                            {
                                var processResult = await mediator.Send(new ProcessEndOfDayQueueCommand
                                {
                                    LogId = prepareResult.Value
                                }, stoppingToken);

                                if (processResult.Value.Status.Equals(EndOfDayLogStatuses.Completed) || processResult.Value.Status.Equals(EndOfDayLogStatuses.CompletedWithErrors))
                                {
                                    _lastExecutionDate = today;
                                    _logger.LogInformation("COB finalizado con éxito. Próxima ejecución permitida: mañana.");
                                }
                                else
                                {
                                    _logger.LogWarning("COB finalizado con estado: {Status}. No se actualizará la última fecha de ejecución para permitir reintentos hoy.", processResult.Value.Status);
                                }
                            }
                        }
                        else
                        {
                            _logger.LogInformation("No es hora de ejecutar COB. Hora actual: {CurrentTime}, Hora programada: {ScheduledTime}", now.TimeOfDay, scheduledTime);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No se encontró una hora de ejecución válida en SystemSettings.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error crítico durante la ejecución del Worker de COB.");
                }

                // 3. Esperar la periodicidad configurada ANTES de la siguiente vuelta
                // Esto va fuera del try-catch para garantizar que siempre se duerma, incluso si hubo error
                await Task.Delay(TimeSpan.FromMinutes(_settings.CheckFrequencyInMinutes), stoppingToken);
            }
        }
    }
}
