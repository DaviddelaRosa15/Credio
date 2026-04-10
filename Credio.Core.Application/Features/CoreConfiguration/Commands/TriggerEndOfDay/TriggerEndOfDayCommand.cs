using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Features.CoreConfiguration.Commands.PrepareEndOfDay;
using Credio.Core.Application.Features.CoreConfiguration.Commands.ProcessEndOfDayQueue;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Features.CoreConfiguration.Commands.TriggerEndOfDay
{
    public class TriggerEndOfDayCommand : ICommand<EndOfDayProcessResponseDTO>
    {
        public bool Reset { get; set; } = false;
    }

    public class TriggerEndOfDayCommandHandler : ICommandHandler<TriggerEndOfDayCommand, EndOfDayProcessResponseDTO>
    {
        private readonly IEndOfDayExecutionLogRepository _endOfDayExecutionLogRepository;
        private readonly ILogger<TriggerEndOfDayCommandHandler> _logger;
        private readonly IEndOfDayService _endOfDayService;

        public TriggerEndOfDayCommandHandler(
            IEndOfDayExecutionLogRepository endOfDayExecutionLogRepository,
            ILogger<TriggerEndOfDayCommandHandler> logger,
            IEndOfDayService endOfDayService)
        {
            _endOfDayExecutionLogRepository = endOfDayExecutionLogRepository;
            _logger = logger;
            _endOfDayService = endOfDayService;
        }

        public async Task<Result<EndOfDayProcessResponseDTO>> Handle(TriggerEndOfDayCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string logId = string.Empty;

                // Obteniendo la fecha actual
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                if (request.Reset)
                {
                    // Si se solicita un reset, se busca el log de ejecución del día actual
                    var resetResult = await _endOfDayExecutionLogRepository.GetByPropertyWithIncludeAsync(e => e.ExecutionDate == today, [e => e.QueueItems]);
                    _logger.LogInformation("Reiniciando el proceso de fin de día para la fecha {Date}. Registro encontrado: {LogId}", today, resetResult?.Id);

                    // Cambiando el estado del log a "Processing" para indicar que se está reiniciando el proceso
                    resetResult.Status = EndOfDayLogStatuses.Processing;

                    if (resetResult.QueueItems.Count > 0)
                    {
                        _logger.LogInformation("Restableciendo {QueueCount} elementos de la cola al estado Pendiente para el registro {LogId}", resetResult.QueueItems.Count, resetResult.Id);

                        // Cambiando el estado de cada item en la cola a "Pending" para que puedan ser procesados nuevamente
                        resetResult.QueueItems.ForEach(q =>
                        {
                            q.Status = EndOfDayQueueStatuses.Pending;
                        });
                    }

                    // Guardando los cambios en la base de datos
                    await _endOfDayExecutionLogRepository.UpdateAsync(resetResult);
                    _logger.LogInformation("El reinicio del proceso de fin de día se completó para la fecha {Date}. El registro {LogId} ahora está en estado de procesamiento.", today, resetResult.Id);
                }

                // Preparando el proceso de fin de día, lo que incluye la creación de un nuevo log de ejecución y la generación de los items en la cola
                try
                {
                    logId = await _endOfDayService.PrepareAsync();
                    _logger.LogInformation("Preparación del proceso de fin de día completada. ID del registro: {LogId}", logId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error al preparar el proceso de fin de día para la fecha {Date}. Error: {ErrorMessage}", today, ex.Message);
                    return Result<EndOfDayProcessResponseDTO>.Failure(Error.InternalServerError("Ocurrió un error al preparar el proceso de fin de día. Por favor, intente nuevamente."));
                }

                try
                {
                    // Si la preparación fue exitosa, se procede a procesar la cola de fin de día utilizando el log ID generado en la preparación
                    var processResult = await _endOfDayService.ProcessQueueAsync(logId);
                    _logger.LogInformation("Procesamiento de la cola de fin de día completado para el registro {LogId}", logId);

                    return Result<EndOfDayProcessResponseDTO>.Success(processResult);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error al procesar la cola de fin de día para el registro {LogId}. Error: {ErrorMessage}", logId, ex.Message);
                    return Result<EndOfDayProcessResponseDTO>.Failure(Error.InternalServerError("Ocurrió un error al procesar la cola de fin de día. Por favor, intente nuevamente."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al desencadenar el proceso de fin de día. Error: {ErrorMessage}", ex.Message);
                return Result<EndOfDayProcessResponseDTO>.Failure(Error.InternalServerError("Ocurrió un error al desencadenar el proceso de fin de día."));
            }
        }
    }
}