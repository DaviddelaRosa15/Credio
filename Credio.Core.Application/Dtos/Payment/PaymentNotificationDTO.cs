namespace Credio.Core.Application.Dtos.Payment
{
    public class PaymentNotificationDTO : PaymentDTO
    {
        public DateTime PaymentDate { get; set; }
        public decimal TotalInterestAppliedAmount { get; set; }
        public decimal TotalLateFeeAppliedAmount { get; set; }
        public decimal TotalPrincipalAppliedAmount { get; set; }
    }
}
