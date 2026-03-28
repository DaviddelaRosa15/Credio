using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Contracts;
using Credio.Core.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.EventHandlers;

public class AdjustLoanDatesEventHandler : IDomainEventHandler<LoanDisbursedEvent>
{
    private readonly ILogger<AdjustLoanDatesEventHandler> _logger;
    private readonly ILoanRepository _loanRepository;

    public AdjustLoanDatesEventHandler(
        ILoanRepository loanRepository,
        ILogger<AdjustLoanDatesEventHandler> logger
        )
    {
        _loanRepository = loanRepository;
        _logger = logger;
    }
    
    public async Task Handle(LoanDisbursedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando actualización de fechas");

            //Obtener el prestamo y sus amortizaciones
            _logger.LogInformation("Consultando prestamo");
            var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == notification.LoanId,
                [loan => loan.AmortizationSchedules]);

            // Calcular la diferencia en días entre la fecha de desembolso y la fecha de creación del préstamo
            int diffDays = notification.DisbursedDate.DayNumber - loan.EffectiveDate.DayNumber;
            _logger.LogInformation($"La diferencia de días del prestamo {loan.LoanNumber} es: {diffDays}");

            if (diffDays > 0) 
            {
                // Ordenar las amortizaciones por número de cuota
                var loanSchedules = loan.AmortizationSchedules.OrderBy(s => s.InstallmentNumber).ToList();

                // Ajustar las fechas de vencimiento de cada amortización sumando la diferencia de días
                loanSchedules.ForEach(s =>
                {
                    s.DueDate = s.DueDate.AddDays(diffDays);
                });

                // Actualizar el préstamo con las nuevas fechas de amortización
                loan.AmortizationSchedules = loanSchedules;

                // Actualizar las fechas de primer pago y vencimiento del préstamo
                loan.FirstPaymentDate = loanSchedules.First().DueDate;
                loan.MaturityDate = loanSchedules.Last().DueDate;

                // Guardar los cambios en la base de datos
                _logger.LogInformation($"Actualizando fechas al prestamo {loan.LoanNumber}");
                await _loanRepository.UpdateAsync(loan);

                _logger.LogInformation("Fechas actualizadas satisfactoriamente");
            }            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar fechas del préstamo {LoanId}", notification.LoanId);
            throw;
        }
    }
}