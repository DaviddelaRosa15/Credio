using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Services
{
    public class AmortizationCalculatorService : IAmortizationCalculatorService
    {
        public List<InstallmentDTO> Calculate(decimal approvedAmount, decimal interestRate, int termInInstallments, 
            DateTime firstPaymentDate, int daysInterval)
        {
            var schedule = new List<InstallmentDTO>(termInInstallments);

            // 1. Calcular la tasa del periodo
            // Se asume año comercial de 360 días, estándar en muchos cálculos de este tipo.
            decimal periodInterestRate = (interestRate / 100m) * (daysInterval / 360m);

            // 2. Calcular la cuota fija (Sistema Francés)
            decimal fixedDueAmount = CalculateFixedInstallment(approvedAmount, periodInterestRate, termInInstallments);

            decimal remainingBalance = approvedAmount;
            DateTime currentPaymentDate = firstPaymentDate;

            // 3. Iterar para generar cada cuota
            for (int i = 1; i <= termInInstallments; i++)
            {
                decimal interestAmount = Math.Round(remainingBalance * periodInterestRate, 2, MidpointRounding.AwayFromZero);
                decimal principalAmount;
                decimal dueAmount;

                // 4. El "Cuadre de Centavos" para la última cuota
                if (i == termInInstallments)
                {
                    // En la última cuota, el abono a capital es exactamente el saldo restante
                    principalAmount = remainingBalance;
                    // Ajustamos la cuota total de este mes para absorber diferencias de centavos
                    dueAmount = principalAmount + interestAmount;
                    remainingBalance = 0m; // Garantizamos que quede en cero absoluto
                }
                else
                {
                    // Cuotas normales
                    dueAmount = fixedDueAmount;
                    principalAmount = dueAmount - interestAmount;
                    remainingBalance -= principalAmount;
                }

                // Registrar la cuota en el cronograma
                schedule.Add(new InstallmentDTO(
                    InstallmentNumber: i,
                    DueDate: currentPaymentDate,
                    DueAmount: dueAmount,
                    PrincipalAmount: principalAmount,
                    InterestAmount: interestAmount,
                    RemainingBalance: remainingBalance
                ));

                // Avanzar la fecha para la siguiente iteración
                currentPaymentDate = currentPaymentDate.AddDays(daysInterval);
            }

            return schedule;
        }

        /// Calcula la cuota fija mensual usando la fórmula de anualidad vencida.
        private decimal CalculateFixedInstallment(decimal principal, decimal periodRate, int periods)
        {
            // Si la tasa es 0 (caso raro pero posible en promociones), la cuota es capital / meses
            if (periodRate == 0) return Math.Round(principal / periods, 2, MidpointRounding.AwayFromZero);

            double rate = (double)periodRate;
            double factor = Math.Pow(1 + rate, periods);

            // Fórmula: R = P * [ i(1+i)^n / ((1+i)^n - 1) ]
            decimal rawInstallment = principal * (decimal)(rate * factor / (factor - 1));

            return Math.Round(rawInstallment, 2, MidpointRounding.AwayFromZero);
        }
    }
}
