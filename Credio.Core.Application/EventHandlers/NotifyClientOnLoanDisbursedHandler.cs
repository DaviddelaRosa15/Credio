using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Helpers;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Contracts;
using Credio.Core.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.EventHandlers;

public class NotifyClientOnLoanDisbursedHandler : IDomainEventHandler<LoanDisbursedEvent>
{
    private readonly IEmailHelper _emailHelper;
    private readonly IEmailService _emailService;
    private readonly ILoanRepository _loanRepository;
    private readonly IPdfService _pdfService;
    private readonly ILogger<NotifyClientOnLoanDisbursedHandler > _logger;

    public NotifyClientOnLoanDisbursedHandler(
        IEmailHelper emailHelper,
        IEmailService emailService,
        ILoanRepository loanRepository,
        IPdfService pdfService,
        ILogger<NotifyClientOnLoanDisbursedHandler > logger
        )
    {
        _emailHelper = emailHelper;
        _emailService = emailService;
        _loanRepository = loanRepository;
        _pdfService = pdfService;
        _logger = logger;
    }
    
    public async Task Handle(LoanDisbursedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Manejando evento de desembolso de préstamo para el préstamo {LoanId}", notification.LoanId);
            
            //Obtener el prestamo y sus amortizaciones
            _logger.LogInformation("Consultando préstamo con ID {LoanId} para obtener información del cliente asociado", notification.LoanId);
            var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == notification.LoanId,
                l => l.Include(l => l.Client).Include(l => l.PaymentFrequency));

            // Verificar si el cliente tiene correo electrónico registrado para enviar la notificación
            if (!string.IsNullOrEmpty(loan?.Client?.Email))
            {
                // Obtener la información necesaria para el correo de notificación
                var dTO = new DisburseLoanNotificationDTO
                {
                    ClientName = $"{loan.Client.FirstName} {loan.Client.LastName}",
                    DocumentNumber = loan.Client.DocumentNumber,
                    EffectiveDate = notification.DisbursedDate,
                    InterestRate = loan.InterestRate,
                    LoanAmount = loan.Amount,
                    LoanId = loan.Id,
                    LoanNumber = loan.LoanNumber,
                    LoanStatus = "Activo",
                    PaymentFrequency = loan.PaymentFrequency.Name,
                    Term = loan.Term
                };

                // Generar el recibo de desembolso en PDF
                var receiptPDF = _pdfService.GenerateDisbursementReceipt(dTO);
                _logger.LogInformation("Recibo de desembolso generado exitosamente para el préstamo {LoanId}", notification.LoanId);

                // Enviar correo de notificación al cliente
                _logger.LogInformation("Enviando correo de notificación de desembolso al cliente {Email}", loan.Client.Email);
                await SendNotificationEmail(dTO, receiptPDF, loan.Client.Email);
                _logger.LogInformation("Correo de notificación de desembolso enviado exitosamente al cliente {Email}", loan.Client.Email);
            }
            else
            {
                _logger.LogInformation("El cliente asociado al préstamo {LoanId} no tiene un correo electrónico registrado.", notification.LoanId);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación de desembolso para el préstamo {LoanId}", notification.LoanId);
            throw;
        }
    }

    private async Task SendNotificationEmail(DisburseLoanNotificationDTO notification, byte[] receiptPDF, string email)
    {
        try
        {
            await _emailService.SendAsync(new EmailRequest()
            {
                To = email,
                Body = _emailHelper.MakeEmailForLoanDisbursement(notification),
                Subject = "Notificación de desembolso realizado",
                Attachments = new List<EmailAttachment>
                {
                    new EmailAttachment
                    {
                        FileName = $"Recibo_DS_{notification.LoanNumber}.pdf",
                        Content = receiptPDF,
                        ContentType = "application/pdf"
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo de notificación de desembolso al cliente {Email}", email);
            throw new Exception("Hubo un error enviando el correo al cliente");
        }
    }
}