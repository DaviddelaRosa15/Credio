namespace Credio.Core.Application.Dtos.Loan
{
    public class DisburseLoanResponseDTO
    {
        public string LoanId { get; set; }
        public double LoanAmount { get; set; }
        public int LoanNumber { get; set; }
        public string LoanStatus { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }
}