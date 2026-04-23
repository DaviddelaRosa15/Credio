using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Credio.Core.Application.Features.Payment.Queries.GetPaymentReceiptPdf
{
    public class GetPaymentReceiptPdfQuery : IQuery<PaymentNotificationDTO>
    {
        public string PaymentId { get; set; } = string.Empty;
    }

    public class GetPaymentReceiptPdfQueryHandler : IQueryHandler<GetPaymentReceiptPdfQuery, PaymentNotificationDTO>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentReceiptPdfQueryHandler(ILoanRepository loanRepository, IPaymentRepository paymentRepository)
        {
            _loanRepository = loanRepository;  
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<PaymentNotificationDTO>> Handle(GetPaymentReceiptPdfQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Consultamos el préstamo con su cliente y frecuencia de pago para obtener la información necesaria para el recibo de desembolso
                var payment = await _paymentRepository.GetByIdWithIncludeAsync(p => p.Id == query.PaymentId,
                    p => p.Include(p => p.PaymentMethod));

                // Si no se encuentra el préstamo, retornamos un error de not found
                if (payment is null) return Result<PaymentNotificationDTO>.Failure(Error.NotFound("No se encontro el pago con el id dado"));

                var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == payment.LoanId,
                    l => l.Include(l => l.Client)
                    .Include(l => l.LoanBalance)
                    .Include(l => l.AmortizationSchedules).ThenInclude(s => s.AmortizationStatus));

                // Obtener la información necesaria para el correo de notificación
                var result = new PaymentNotificationDTO
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
                    TotalInterestAppliedAmount = 0,
                    TotalLateFeeAppliedAmount = 0,
                    TotalPrincipalAppliedAmount = 0
                };

                return Result<PaymentNotificationDTO>.Success(result);
            }
            catch
            {
                return Result<PaymentNotificationDTO>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando el préstamo"));
            }
        }
    }
}