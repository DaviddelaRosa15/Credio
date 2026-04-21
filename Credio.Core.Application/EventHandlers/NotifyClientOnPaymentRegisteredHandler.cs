using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Interfaces.Helpers;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Contracts;
using Credio.Core.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.EventHandlers;

public class NotifyClientOnPaymentRegisteredHandler : IDomainEventHandler<PaymentRegisteredEvent>
{
    private readonly IEmailHelper _emailHelper;
    private readonly IEmailService _emailService;
    private readonly ILoanRepository _loanRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPdfService _pdfService;
    private readonly ILogger<NotifyClientOnPaymentRegisteredHandler > _logger;

    public NotifyClientOnPaymentRegisteredHandler(
        IEmailHelper emailHelper,
        IEmailService emailService,
        IPaymentRepository paymentRepository,
        ILoanRepository loanRepository,
        IPdfService pdfService,
        ILogger<NotifyClientOnPaymentRegisteredHandler > logger
        )
    {
        _emailHelper = emailHelper;
        _emailService = emailService;
        _paymentRepository = paymentRepository;
        _loanRepository = loanRepository;
        _pdfService = pdfService;
        _logger = logger;
    }
    
    public async Task Handle(PaymentRegisteredEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Manejando evento de pago registrado para el pago {PaymentId}", notification.PaymentId);

            //Obtener el prestamo y sus amortizaciones
            _logger.LogInformation("Consultando pago con ID {PaymentId} para obtener información del cliente asociado", notification.PaymentId);
            var payment = await _paymentRepository.GetByIdWithIncludeAsync(p => p.Id == notification.PaymentId,
                p => p.Include(p => p.PaymentMethod));

            var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == payment.LoanId,
                l => l.Include(l => l.Client)
                .Include(l => l.LoanBalance)
                .Include(l => l.AmortizationSchedules).ThenInclude(s => s.AmortizationStatus));

            // Verificar si el cliente tiene correo electrónico registrado para enviar la notificación
            if (!string.IsNullOrEmpty(loan?.Client?.Email))
            {
                // Obtener la información necesaria para el correo de notificación
                var dTO = new PaymentNotificationDTO
                {
                    PaymentId = payment.Id,
                    ReceiptNumber = payment.ReceiptNumber,
                    PaymentDate = (DateTime)payment.PaymentDate,
                    ClientName = $"{loan.Client.FirstName} {loan.Client.LastName}",
                    LoanNumber = loan.LoanNumber,
                    PaymentMethod = payment.PaymentMethod.Name,
                    AmountPaid = payment.AmountPaid,
                    RemainingBalance = loan.LoanBalance.PrincipalBalance,
                    PaidInstallmentsCount = loan.AmortizationSchedules.Count(s => s.AmortizationStatus.Name == "Pagada"),
                    TotalInstallmentsCount = loan.AmortizationSchedules.Count(),
                    TotalInterestAppliedAmount = notification.TotalInterestAppliedAmount,
                    TotalLateFeeAppliedAmount = notification.TotalLateFeeAppliedAmount,
                    TotalPrincipalAppliedAmount = notification.TotalPrincipalAppliedAmount
                };

                // Generar el recibo de pago en PDF
                var receiptPDF = _pdfService.GeneratePaymentReceipt(dTO);
                _logger.LogInformation("Recibo de pago generado exitosamente para el pago {PaymentId}", notification.PaymentId);

                // Enviar correo de notificación al cliente
                _logger.LogInformation("Enviando correo de notificación de pago al cliente {Email}", loan.Client.Email);
                await SendNotificationEmail(dTO, receiptPDF, loan.Client.Email);
                _logger.LogInformation("Correo de notificación de pago enviado exitosamente al cliente {Email}", loan.Client.Email);
            }
            else
            {
                _logger.LogInformation("El cliente asociado al préstamo {PaymentId} no tiene un correo electrónico registrado.", notification.PaymentId);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar fechas del préstamo {PaymentId}", notification.PaymentId);
            throw;
        }
    }

    private async Task SendNotificationEmail(PaymentNotificationDTO notification, byte[] receiptPDF, string email)
    {
        try
        {
            await _emailService.SendAsync(new EmailRequest()
            {
                To = email,
                Body = _emailHelper.MakeEmailForPaymentNotifications(notification),
                Subject = "Notificación de pago realizado",
                Attachments = new List<EmailAttachment>
                {
                    new EmailAttachment
                    {
                        FileName = $"Recibo_{notification.ReceiptNumber.Replace("-", "_")}.pdf",
                        Content = receiptPDF,
                        ContentType = "application/pdf"
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo de notificación de pago al cliente {Email}", email);
            throw new Exception("Hubo un error enviando el correo al cliente");
        }
    }
}