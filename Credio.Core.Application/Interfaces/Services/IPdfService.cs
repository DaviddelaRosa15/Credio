using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Dtos.Payment;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IPdfService
    {
        byte[] GenerateDisbursementReceipt(DisburseLoanNotificationDTO data);
        byte[] GeneratePaymentReceipt(PaymentNotificationDTO data);
    }
}
