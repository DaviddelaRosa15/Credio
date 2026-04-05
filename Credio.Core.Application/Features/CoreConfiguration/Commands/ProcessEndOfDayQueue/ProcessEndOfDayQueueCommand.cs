using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Core.Domain.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credio.Core.Application.Features.CoreConfiguration.Commands.ProcessEndOfDayQueue
{
    public class ProcessEndOfDayQueueCommand : ICommand
    {
        public string LogId { get; set; }
    }

    public class ProcessEndOfDayQueueCommandHandler : ICommandHandler<ProcessEndOfDayQueueCommand>
    {
        private readonly IAmortizationStatusRepository _amortizationStatusRepository;
        private readonly IEndOfDayExecutionLogRepository _endOfDayExecutionLogRepository;
        private readonly IEndOfDayQueueRepository _endOfDayQueueRepository;
        private readonly EndOfDayLogSettings _endOfDayLogSettings;
        private readonly ILoanStatusRepository _loanStatusRepository;
        private readonly ILogger<ProcessEndOfDayQueueCommandHandler> _logger;
        private readonly ISystemSettingsRepository _settingsRepository;

        public ProcessEndOfDayQueueCommandHandler(
            IAmortizationStatusRepository amortizationStatusRepository,
            IEndOfDayExecutionLogRepository endOfDayExecutionLogRepository,
            IEndOfDayQueueRepository endOfDayQueueRepository,
            IOptions<EndOfDayLogSettings> endOfDayLogSettings,
            ILoanStatusRepository loanStatusRepository,
            ILogger<ProcessEndOfDayQueueCommandHandler> logger,
            ISystemSettingsRepository settingsRepository)
        {
            _amortizationStatusRepository = amortizationStatusRepository;
            _endOfDayExecutionLogRepository = endOfDayExecutionLogRepository;
            _endOfDayQueueRepository = endOfDayQueueRepository;
            _endOfDayLogSettings = endOfDayLogSettings.Value;
            _loanStatusRepository = loanStatusRepository;
            _logger = logger;
            _settingsRepository = settingsRepository;
        }

        public async Task<Result> Handle(ProcessEndOfDayQueueCommand request, CancellationToken cancellationToken)
        {
            int processedLoans = 0;

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
            var statusInArrears = await _loanStatusRepository.GetByPropertyAsync(s => s.Name.Equals("En Mora"));

            // Obteniendo el registro de ejecución de COB para el día de hoy
            var log = await _endOfDayExecutionLogRepository.GetByIdAsync(request.LogId);

            try
            {
                _logger.LogInformation($"Iniciando proceso de COB para el día de hoy. Log ID: {request.LogId}");
                int loteNumber = 1;
                int pageSize = _endOfDayLogSettings.BatchSize;

                // Obteniendo los registros de la cola de COB para el día de hoy, esto con el fin de procesar los préstamos vencidos en lotes
                var queues = await _endOfDayQueueRepository.GetPagedAsync(1, pageSize,
                    e => e.Include(e => e.Loan). ThenInclude(l => l.AmortizationSchedules).ThenInclude(a => a.LateFees)
                    .Include(e => e.Loan).ThenInclude(l => l.LoanBalance),
                    e => e.LogId == request.LogId && e.Status == EndOfDayQueueStatuses.Pending
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
                                }
                                else
                                {
                                    // Se crea el registro de interés moratorio para el día de hoy
                                    schedule.LateFees.Add(new LateFee
                                    {
                                        Amount = (double)lateFeeTotal,
                                        AmortizationScheduleId = schedule.Id,
                                        Balance = (double)lateFeeTotal,
                                        GeneratedDate = DateTime.Now,
                                        LoanId = item.Loan.Id
                                    });

                                    // Se acumulan los intereses moratorios totales para el préstamo
                                    totalLateFees += lateFeeTotal;
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
                        catch(Exception ex)
                        {
                            _logger.LogError($"Error al procesar el préstamo con ID {item.LoanId}: {ex}");
                            item.Status = EndOfDayQueueStatuses.Failed;
                            item.ErrorMessage = $"Ocurrio un error al procesar el préstamo: {ex.Message}";
                            item.ProcessedAt = DateTime.Now;
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
                    e => e.LogId == request.LogId && e.Status == EndOfDayQueueStatuses.Pending
                    );
                }

                _logger.LogInformation($"Proceso de COB para el día de hoy completado. Total de préstamos procesados: {processedLoans}");
                log.Status = EndOfDayLogStatuses.Completed;
                log.ProcessedLoans = processedLoans;
                log.EndTime = DateTime.Now;
                log.Notes = $"Proceso de COB completado para el día de hoy. Total de préstamos procesados: {processedLoans}";

                // Se actualiza el registro de ejecución de COB para el día de hoy con el estado "Completado" y las notas correspondientes al proceso realizado
                _logger.LogInformation($"Actualizando registro de ejecución de COB para el día de hoy con el estado 'Completado'. Log ID: {request.LogId}");
                await _endOfDayExecutionLogRepository.UpdateAsync(log);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error procesando la cola de COB para el día de hoy. Log ID: {request.LogId}. Error: {ex}");
                return Result.Failure(Error.InternalServerError("Ocurrio un error al procesar la cola de COB para el día de hoy."));
            }
        }
    }
}