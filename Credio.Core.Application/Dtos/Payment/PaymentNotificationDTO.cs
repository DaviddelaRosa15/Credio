namespace Credio.Core.Application.Dtos.Payment
{
    public class PaymentNotificationDTO : PaymentDTO
    {
        public decimal TotalInterestAppliedAmount { get; set; }
        public decimal TotalLateFeeAppliedAmount { get; set; }
        public decimal TotalPrincipalAppliedAmount { get; set; }
    }
}
