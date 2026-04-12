using Credio.Core.Application.Features.Payment.Commands.RegisterPayment;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task ProcessPaymentAsync(string loanId, Payment payment);
        Task<Payment> RegisterInitialPaymentAsync(RegisterPaymentCommand command);
    }
}