using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Contracts;
using Credio.Core.Domain.Entities;
using Credio.Core.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.EventHandlers;

public class CreateInitialLoanBalanceEventHandler : IDomainEventHandler<LoanDisbursedEvent>
{
    private readonly ILogger<CreateInitialLoanBalanceEventHandler> _logger;
    private readonly ILoanRepository _loanRepository;
    private readonly ILoanBalanceRepository _loanBalanceRepository;

    public CreateInitialLoanBalanceEventHandler(
        ILoanBalanceRepository loanBalanceRepository,
        ILoanRepository loanRepository,
        ILogger<CreateInitialLoanBalanceEventHandler> logger
        )
    {
        _loanBalanceRepository = loanBalanceRepository;
        _loanRepository = loanRepository;
        _logger = logger;
    }
    
    public async Task Handle(LoanDisbursedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando registro de balance");

            //Obtener el prestamo
            _logger.LogInformation("Consultando prestamo");
            var loan = await _loanRepository.GetByIdAsync(notification.LoanId);

            // Instanciar el balance inicial del préstamo
            LoanBalance balance = new()
            {
                LoanId = notification.LoanId,
                TotalOutstanding = loan.Amount,
                PrincipalBalance = loan.Amount,
                InterestBalance = 0,
                LateFeeBalance = 0,
                DaysInArrears = 0,
                UpdatedAt = DateTime.Now
            };

            // Guardar el balance inicial en la base de datos
            _logger.LogInformation($"Registrando balance de prestamo {loan.LoanNumber}");
            await _loanBalanceRepository.AddAsync(balance);

            _logger.LogInformation("Balance registrado satisfactoriamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar balance para el préstamo {LoanId}", notification.LoanId);
            throw;
        }
    }
}