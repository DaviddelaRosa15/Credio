using Credio.Core.Application.Dtos.Common;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IAmortizationCalculatorService
    {
        List<InstallmentDTO> Calculate(decimal approvedAmount, decimal interestRate, int termInInstallments,
            DateOnly firstPaymentDate, int daysInterval);
    }
}
