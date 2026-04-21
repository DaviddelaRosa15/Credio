using Credio.Core.Application.Dtos.Payment;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IPdfService
    {
        byte[] GeneratePaymentReceipt(PaymentNotificationDTO data);
    }
}
