using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Services
{
    public class ReceiptNumberGeneratorService : IReceiptNumberGeneratorService
    {
        private readonly IPaymentRepository _paymentRepository;

        public ReceiptNumberGeneratorService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<string> GenerateReceiptNumberAsync(string loanNumber, string loanId)
        {

            // Obtener todos los pagos asociados al préstamo específico utilizando el LoanId
            var payments = await _paymentRepository.GetAllByPropertyAsync(p => p.LoanId == loanId);

            // 1. Contar cuántos pagos existen actualmente para este préstamo específico
            int previousPaymentsCount = payments.Count();

            // 2. Calcular el siguiente número en la secuencia
            int nextSequentialNumber = previousPaymentsCount + 1;

            // 3. Formatear el número con ceros a la izquierda (máximo 4 dígitos)
            //    El formato "D4" asegura que el número 1 se convierta en "0001", el 12 en "0012", etc.
            string formattedSequential = nextSequentialNumber.ToString("D4");

            // 4. Construir y retornar el número de recibo final
            return $"REC-P{loanNumber}-{formattedSequential}";
        }
    }
}