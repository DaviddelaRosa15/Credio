using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Interfaces.Helpers;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Contracts;
using Credio.Core.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.EventHandlers;

public class NotifyClientOnArrearsHandler : IDomainEventHandler<LoanEnteredArrearsEvent>
{
    private readonly IEmailHelper _emailHelper;
    private readonly IEmailService _emailService;
    private readonly ILoanRepository _loanRepository;
    private readonly ILogger<NotifyClientOnArrearsHandler> _logger;

    public NotifyClientOnArrearsHandler(
        IEmailHelper emailHelper,
        IEmailService emailService,
        ILoanRepository loanRepository,
        ILogger<NotifyClientOnArrearsHandler> logger
        )
    {
        _emailHelper = emailHelper;
        _emailService = emailService;
        _loanRepository = loanRepository;
        _logger = logger;
    }
    
    public async Task Handle(LoanEnteredArrearsEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Manejando evento de préstamo en mora para el préstamo {LoanId}", notification.LoanId);

            //Obtener el prestamo y sus amortizaciones
            _logger.LogInformation("Consultando prestamo");
            var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == notification.LoanId,
                [loan => loan.Client]);

            // Verificar si el cliente tiene correo electrónico registrado para enviar la notificación
            if (!string.IsNullOrEmpty(loan?.Client?.Email))
            {
                LoanInArrearsNotificationDTO dTO = new LoanInArrearsNotificationDTO()
                {
                    ArrearsAmount = notification.Amount,
                    DaysOverdue = notification.DaysInArrears,
                    ClientName = $"{loan.Client.FirstName} {loan.Client.LastName}",
                    LoanNumber = loan.LoanNumber,
                    TotalDue = notification.TotalAmount
                };

                // Enviar correo de notificación al cliente
                _logger.LogInformation("Enviando correo de notificación de préstamo en mora al cliente {Email}", loan.Client.Email);
                await SendNotificationEmail(dTO, loan.Client.Email);
                _logger.LogInformation("Correo de notificación de préstamo en mora enviado exitosamente al cliente {Email}", loan.Client.Email);
            }
            else
            {
                _logger.LogInformation("El cliente asociado al préstamo {LoanId} no tiene un correo electrónico registrado.", notification.LoanId);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar fechas del préstamo {LoanId}", notification.LoanId);
            throw;
        }
    }

    private async Task SendNotificationEmail(LoanInArrearsNotificationDTO notification, string email)
    {
        try
        {
            await _emailService.SendAsync(new EmailRequest()
            {
                To = email,
                Body = _emailHelper.MakeEmailForLoanArrearsNotice(notification),
                Subject = "Notificación de préstamo en mora"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo de notificación de préstamo en mora al cliente {Email}", email);
            throw new Exception("Hubo un error enviando el correo al cliente");
        }
    }
}