using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Features.Payment.Commands.RegisterPayment;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<RegisterPaymentResponseDTO> GetPaymentReceiptSnapshotAsync(string paymentId);
        Task ProcessPaymentAsync(string loanId, Payment payment);
        Task<Payment> RegisterInitialPaymentAsync(RegisterPaymentCommand command, string collectorId, int loanNumber);
    }
}