namespace Credio.Core.Application.Dtos.Payment
{
    public class PaymentSearchDTO
    {
        public DateOnly DueDate { get; set; }
        public string FullName { get; set; }
        public int InstallmentNumber { get; set; }
        public string InstallmentStatus { get; set; }
        public int LoanNumber { get; set; }
        public decimal TotalDueAmount { get; set; }
    }
}