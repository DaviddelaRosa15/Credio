namespace Credio.Core.Application.Dtos.Payment
{
    public class PaymentDistributionState
    {
        public bool ApplyExtraToPrincipal { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal TotalAppliedAmount { get; set; }
        public decimal TotalInterestAppliedAmount { get; set; }
        public decimal TotalLateFeeAppliedAmount { get; set; }
        public decimal TotalPrincipalAppliedAmount { get; set; }
    }
}