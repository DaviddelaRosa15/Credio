using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Entities;
using Credio.Core.Domain.Events;
using Credio.Core.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credio.Core.Application.Services
{
    public class EndOfDayService : IEndOfDayService
    {
        private readonly IAmortizationStatusRepository _amortizationStatusRepository;
        private readonly IEndOfDayExecutionLogRepository _endOfDayExecutionLogRepository;
        private readonly IEndOfDayQueueRepository _endOfDayQueueRepository;
        private readonly EndOfDayLogSettings _endOfDayLogSettings;
        private readonly ILateFeeRepository _lateFeeRepository;
        private readonly ILateFeeStatusRepository _lateFeeStatusRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanStatusRepository _loanStatusRepository;
        private readonly ILogger<EndOfDayService> _logger;
        private readonly ISystemSettingsRepository _settingsRepository;

        public EndOfDayService(
            IAmortizationStatusRepository amortizationStatusRepository,
            IEndOfDayExecutionLogRepository endOfDayExecutionLogRepository,
            IEndOfDayQueueRepository endOfDayQueueRepository,
            IOptions<EndOfDayLogSettings> endOfDayLogSettings,
            ILateFeeRepository lateFeeRepository,
            ILateFeeStatusRepository lateFeeStatusRepository,
            ILoanRepository loanRepository,
            ILoanStatusRepository loanStatusRepository,
            ILogger<EndOfDayService> logger,
            ISystemSettingsRepository settingsRepository)
        {
            _amortizationStatusRepository = amortizationStatusRepository;
            _endOfDayExecutionLogRepository = endOfDayExecutionLogRepository;
            _endOfDayQueueRepository = endOfDayQueueRepository;
            _endOfDayLogSettings = endOfDayLogSettings.Value;
            _lateFeeRepository = lateFeeRepository;
            _lateFeeStatusRepository = lateFeeStatusRepository;
            _loanRepository = loanRepository;
            _loanStatusRepository = loanStatusRepository;
            _logger = logger;
            _settingsRepository = settingsRepository;
        }

        public async Task<string> PrepareAsync()
        {
            string logId = string.Empty;

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
                        logId = createdLog.Id;
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

                        // Actualizando el registro de COB con el total de préstamos a procesar
                        createdLog.TotalLoans = queueItems.Count;
                        await _endOfDayExecutionLogRepository.UpdateAsync(createdLog);

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
                    logId = existinTodayCOB.Id;
                }

                return logId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el proceso: {ex}");
                throw new Exception("Ocurrio un error al preparar el proceso de COB para el dia de hoy");
            }
        }

        public async Task<EndOfDayProcessResponseDTO> ProcessQueueAsync(string logId)
        {
            bool hasErrors = false;
            int processedLoans = 0;
            int failedLoans = 0;

            // Obteniendo la fecha actual
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            // Obteniendo el periodo de gracia para el proceso de COB, esto con el fin de incluir en el proceso aquellos prestamos que tengan cuotas vencidas pero que se encuentren dentro del periodo de gracia
            var settingGracePeriod = await _settingsRepository.GetByPropertyAsync(s => s.Key.Equals(EndOfDaySettings.GracePeriodDaysKey));
            int gracePeriod = int.Parse(settingGracePeriod.Value);

            // Obteniendo la tasa de interes moratorio diario para el proceso de COB, esto con el fin de calcular los intereses moratorios correspondientes a las cuotas vencidas que se encuentren fuera del periodo de gracia
            var settingLateFeeRate = await _settingsRepository.GetByPropertyAsync(s => s.Key.Equals(EndOfDaySettings.DailyLateFeeRateKey));
            decimal dailyLateFeeRate = decimal.Parse(settingLateFeeRate.Value);

            // Obteniendo el estado de amortización que corresponde a "Atrasada", esto con el fin de actualizar el estado de las cuotas vencidas que se encuentren fuera del periodo de gracia
            var statusOverdue = await _amortizationStatusRepository.GetByPropertyAsync(s => s.Name.Equals("Atrasada"));

            // Obteniendo el estado de amortización que corresponde a "Pagada"
            var statusPaid = await _amortizationStatusRepository.GetByPropertyAsync(s => s.Name.Equals("Pagada"));

            // Obteniendo el estado de préstamo que corresponde a "En Mora"
            var statusInArrears = await _loanStatusRepository.GetByPropertyAsync(s => s.Name.Equals("En mora"));

            // Obteniendo el estado de mora pendiente
            var statusFeePending = await _lateFeeStatusRepository.GetByPropertyAsync(s => s.Name.Equals("Pendiente"));

            // Obteniendo el registro de ejecución de COB para el día de hoy
            var log = await _endOfDayExecutionLogRepository.GetByIdAsync(logId);

            try
            {
                _logger.LogInformation($"Iniciando proceso de COB para el día de hoy. Log ID: {logId}");
                int loteNumber = 1;
                int pageSize = _endOfDayLogSettings.BatchSize;

                // Se actualiza el estado del registro de ejecución de COB para el día de hoy a "En Proceso" para indicar que el proceso de COB ha iniciado
                if (log.Status != EndOfDayLogStatuses.Processing)
                {
                    _logger.LogInformation($"Actualizando estado del registro de ejecución de COB para el día de hoy a 'En Proceso'. Log ID: {logId}");
                    log.Status = EndOfDayLogStatuses.Processing;
                    await _endOfDayExecutionLogRepository.UpdateAsync(log);
                }

                // Obteniendo los registros de la cola de COB para el día de hoy, esto con el fin de procesar los préstamos vencidos en lotes
                var queues = await _endOfDayQueueRepository.GetPagedAsync(1, pageSize,
                    e => e.Include(e => e.Loan).ThenInclude(l => l.AmortizationSchedules).ThenInclude(a => a.LateFees)
                    .Include(e => e.Loan).ThenInclude(l => l.LoanBalance),
                    e => e.LogId == logId && e.Status == EndOfDayQueueStatuses.Pending
                );

                // Se itera mientras existan registros en la cola de COB para el día de hoy, esto con el fin de procesar todos los préstamos vencidos
                while (queues.Items.Any())
                {
                    _logger.LogInformation($"Procesando lote {loteNumber} de COB para el día de hoy. Total de préstamos en el lote: {queues.Items.Count}. Total de préstamos procesados hasta el momento: {processedLoans}");
                    List<EndOfDayQueue> logs = queues.Items;

                    // Procesar cada préstamo vencido
                    foreach (var item in logs)
                    {
                        try
                        {
                            decimal totalLateFees = 0;

                            // Procesar cada cuota del préstamo para verificar si se encuentra vencida y calcular los intereses moratorios correspondientes
                            foreach (var schedule in item.Loan.AmortizationSchedules.Where(a => (a.DueDate.AddDays(gracePeriod) < today) && a.AmortizationStatusId != statusPaid.Id))
                            {
                                // Calculo de la diferencia en días entre la fecha de vencimiento de la cuota y la fecha actual 
                                int diffDays = today.DayNumber - schedule.DueDate.DayNumber;

                                // Monto a calcular intereses moratorios
                                decimal amountDue = schedule.DueAmount - (schedule.PaidAmount ?? 0);

                                // Calculo de intereses moratorios totales
                                decimal lateFeeTotal = amountDue * dailyLateFeeRate * diffDays;

                                // Calculo de intereses moratorios de hoy
                                decimal lateFee = amountDue * dailyLateFeeRate;

                                // Actualización del estado de la cuota a "Atrasada"
                                schedule.AmortizationStatusId = statusOverdue.Id;

                                // Obtener el registro de interés moratorio activo para la cuota, esto con el fin de actualizarlo si ya existe o crear uno nuevo si no existe
                                var activeLateFee = schedule.LateFees.FirstOrDefault(lf => lf.Balance != 0);

                                if (activeLateFee is not null)
                                {
                                    // Se actualiza el registro de interés moratorio para el día de hoy
                                    activeLateFee.Amount += (double)lateFee;
                                    activeLateFee.Balance += (double)lateFee;

                                    // Se acumulan los intereses moratorios totales para el préstamo
                                    totalLateFees += lateFee;

                                    // Se actualiza el número de días en mora para el préstamo
                                    item.Loan.LoanBalance.DaysInArrears += 1;
                                }
                                else
                                {
                                    LateFee newLateFee = new()
                                    {
                                        Amount = (double)lateFeeTotal,
                                        AmortizationScheduleId = schedule.Id,
                                        Balance = (double)lateFeeTotal,
                                        GeneratedDate = DateTime.Now,
                                        LoanId = item.Loan.Id,
                                        LateFeeStatusId = statusFeePending.Id
                                    };

                                    // Se agrega el evento de dominio de préstamo entró en mora al nuevo registro de interés moratorio
                                    newLateFee.AddEvent(new LoanEnteredArrearsEvent(item.Loan.Id, (double)lateFeeTotal, diffDays));

                                    // Se crea el registro de interés moratorio para el día de hoy
                                    await _lateFeeRepository.AddAsync(newLateFee);

                                    // Se acumulan los intereses moratorios totales para el préstamo
                                    totalLateFees += lateFeeTotal;

                                    // Se actualiza el número de días en mora para el préstamo con la diferencia en días calculada
                                    item.Loan.LoanBalance.DaysInArrears += diffDays;
                                }
                            }

                            // Actualización del balance del préstamo con los intereses moratorios generados en el proceso de COB
                            item.Loan.LoanBalance.LateFeeBalance += (double)totalLateFees;
                            item.Loan.LoanBalance.TotalOutstanding += (double)totalLateFees;

                            // Actualización del estado del préstamo a "En Mora" si se generaron intereses moratorios en el proceso de COB
                            item.Loan.LoanStatusId = statusInArrears.Id;
                            item.Status = EndOfDayQueueStatuses.Processed;
                            item.ProcessedAt = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error al procesar el préstamo con ID {item.LoanId}: {ex}");
                            item.Status = EndOfDayQueueStatuses.Failed;
                            item.ErrorMessage = $"Ocurrio un error al procesar el préstamo: {ex.Message}";
                            item.ProcessedAt = DateTime.Now;

                            failedLoans++;
                            hasErrors = true;

                            continue;
                        }
                    }

                    // Se actualizan los registros de la cola de COB para el día de hoy con el estado "Procesado" y los cambios realizados en el proceso de COB
                    _logger.LogInformation($"Actualizando registros de la cola de COB para el lote {loteNumber} con el estado 'Procesado'. Total de préstamos a actualizar: {logs.Count}");
                    await _endOfDayQueueRepository.UpdateManyAsync(logs);

                    loteNumber++;
                    processedLoans += logs.Count;
                    queues = await _endOfDayQueueRepository.GetPagedAsync(1, pageSize,
                    e => e.Include(e => e.Loan).ThenInclude(l => l.AmortizationSchedules).ThenInclude(a => a.LateFees)
                    .Include(e => e.Loan).ThenInclude(l => l.LoanBalance),
                    e => e.LogId == logId && e.Status == EndOfDayQueueStatuses.Pending
                    );
                }

                _logger.LogInformation($"Proceso de COB para el día de hoy completado. Total de préstamos procesados: {processedLoans}");
                log.Status = hasErrors ? EndOfDayLogStatuses.CompletedWithErrors : EndOfDayLogStatuses.Completed;
                log.ProcessedLoans = processedLoans;
                log.EndTime = DateTime.Now;
                log.Notes = $"Proceso de COB completado para el día de hoy. Total de préstamos procesados: {processedLoans}";

                // Se actualiza el registro de ejecución de COB para el día de hoy con el estado "Completado" y las notas correspondientes al proceso realizado
                _logger.LogInformation($"Actualizando registro de ejecución de COB para el día de hoy con el estado 'Completado'. Log ID: {logId}");
                await _endOfDayExecutionLogRepository.UpdateAsync(log);

                return new EndOfDayProcessResponseDTO
                {
                    ExecutionTime = log.StartTime,
                    FailedLoans = failedLoans,
                    LogId = logId,
                    ProcessedLoans = processedLoans,
                    Status = hasErrors ? EndOfDayLogStatuses.CompletedWithErrors : EndOfDayLogStatuses.Completed,
                    TotalLoans = processedLoans + failedLoans
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error procesando la cola de COB para el día de hoy. Log ID: {logId}. Error: {ex}");

                // Se actualiza el registro de ejecución de COB para el día de hoy con el estado "Fallido"
                log.Status = EndOfDayLogStatuses.Failed;
                await _endOfDayExecutionLogRepository.UpdateAsync(log);

                return new EndOfDayProcessResponseDTO
                {
                    ExecutionTime = log.StartTime,
                    FailedLoans = failedLoans,
                    LogId = logId,
                    ProcessedLoans = processedLoans,
                    Status = EndOfDayLogStatuses.Failed,
                    TotalLoans = processedLoans + failedLoans
                };
            }
        }
    }
}