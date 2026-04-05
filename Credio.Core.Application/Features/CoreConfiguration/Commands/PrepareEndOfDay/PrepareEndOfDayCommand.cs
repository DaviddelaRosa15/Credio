using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Features.CoreConfiguration.Commands.PrepareEndOfDay
{
    public class PrepareEndOfDayCommand : ICommand
    {

    }

    public class PrepareEndOfDayCommandHandler : ICommandHandler<PrepareEndOfDayCommand>
    {
        private readonly IEndOfDayExecutionLogRepository _endOfDayExecutionLogRepository;
        private readonly IEndOfDayQueueRepository _endOfDayQueueRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILogger<PrepareEndOfDayCommandHandler> _logger;

        public PrepareEndOfDayCommandHandler(
            IEndOfDayExecutionLogRepository endOfDayExecutionLogRepository,
            IEndOfDayQueueRepository endOfDayQueueRepository,
            ILoanRepository loanRepository,
            ILogger<PrepareEndOfDayCommandHandler> logger)
        {
            _endOfDayExecutionLogRepository = endOfDayExecutionLogRepository;
            _endOfDayQueueRepository = endOfDayQueueRepository;
            _loanRepository = loanRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(PrepareEndOfDayCommand request, CancellationToken cancellationToken)
        {
            // Obteniendo la fecha actual
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            try
            {
                // Verificando si ya existe un registro de COB para el día de hoy
                var existinTodayCOB = await _endOfDayExecutionLogRepository.GetByPropertyAsync(e => e.ExecutionDate == today);

                if (existinTodayCOB == null)
                {
                    // Creando un nuevo registro de COB para el día de hoy
                    EndOfDayExecutionLog newLog = new EndOfDayExecutionLog
                    {
                        ExecutionDate = today,
                        StartTime = DateTime.Now,
                        Status = EndOfDayLogStatuses.NotStarted,
                        TotalLoans = 0,
                        ProcessedLoans = 0
                    };

                    // Variable para almacenar el registro creado, se declara aquí para poder usarla después de la creación
                    EndOfDayExecutionLog createdLog;

                    try
                    {
                        // Si no existe, se crea un nuevo registro de COB para el día de hoy
                        _logger.LogInformation($"Creando un nuevo registro de COB para el dia de hoy ({today})");
                        createdLog = await _endOfDayExecutionLogRepository.AddAsync(newLog);
                        _logger.LogInformation($"Registro de COB creado con ID: {createdLog.Id}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error creando el el registro de COB para el dia de hoy ({today}): {ex.Message}");
                    }

                    try
                    {
                        // Obteniendo los prestamos que tienen cuotas vencidas para el dia de hoy
                        var overdueLoans = await _loanRepository.GetAllByPropertyWithIncludeAsync(l => l.AmortizationSchedules.Any(a => a.DueDate < today),
                            [l => l.AmortizationSchedules]);
                        _logger.LogInformation($"Se encontraron {overdueLoans.Count} prestamos con cuotas vencidas para el dia de hoy ({today})");

                        // Creando los registros en la cola de COB para cada cuota vencida
                        List<EndOfDayQueue> queueItems = overdueLoans.Select(loan => new EndOfDayQueue
                            {
                                LoanId = loan.Id,
                                LogId = createdLog.Id,
                                Status = EndOfDayQueueStatuses.Pending
                            }).ToList();

                        // Guardando los registros en la base de datos
                        _logger.LogInformation($"Creando {queueItems.Count} registros en la cola de COB para el dia de hoy ({today})");
                        var createdQueueItems = await _endOfDayQueueRepository.AddManyAsync(queueItems);
                        _logger.LogInformation($"Registros en la cola de COB creados para el dia de hoy ({today})");

                        _logger.LogInformation($"Preparación completada: {queueItems.Count} préstamos listos para procesar.");
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre un error al crear los registros en la cola de COB, se actualiza el registro de COB con el estado de "Failed" y se lanza una excepción
                        createdLog.Status = EndOfDayLogStatuses.Failed;

                        // Se intenta actualizar el registro de COB con el estado de "Failed", si ocurre un error al actualizar, se lanza una excepción con el error original y el error al actualizar
                        await _endOfDayExecutionLogRepository.UpdateAsync(createdLog);

                        throw new Exception($"Error creando los registros en la cola de COB para el dia de hoy ({today}): {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Ya existe un registro de COB para el dia de hoy ({today}) con ID: {existinTodayCOB.Id}");
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el proceso: {ex}");
                return Result.Failure(Error.InternalServerError("Ocurrio un error al preparar el proceso de COB para el dia de hoy"));
            }
        }
    }
}