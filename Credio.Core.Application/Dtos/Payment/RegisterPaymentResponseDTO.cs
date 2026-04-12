namespace Credio.Core.Application.Dtos.Payment
{
    public class RegisterPaymentResponseDTO
    {
        // Datos de Identificación y Auditoría
        public string PaymentId { get; set; }
        public string ReceiptNumber { get; set; }
        public DateOnly PaymentDate { get; set; }

        // Información del Cliente y Préstamo
        public string ClientName { get; set; }
        public int LoanNumber { get; set; }
        public string PaymentMethod { get; set; }

        // Datos Financieros (Post-Cascada)
        public double AmountPaid { get; set; }
        public double RemainingBalance { get; set; } 

        // Progreso de Cuotas (El "7/12")
        public int PaidInstallmentsCount { get; set; } // 7
        public int TotalInstallmentsCount { get; set; } // 12
    }
}
