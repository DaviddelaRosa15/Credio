namespace Credio.Core.Application.Dtos.Loan
{
    public class DisburseLoanNotificationDTO : DisburseLoanResponseDTO
    {
        public string ClientName { get; set; }
        public string DocumentNumber { get; set; }
        public double InterestRate { get; set; }
        public int Term { get; set; }
        public string PaymentFrequency { get; set; }
    }
}