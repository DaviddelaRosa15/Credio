using Credio.Core.Application.Dtos.Loan;

namespace Credio.Core.Application.Features.Loan.Queries.GetAllLoans
{
    public class GetAllLoansQueryResponse : LoanDTO
    {
        public DateOnly? DisbursedDate { get; set; }
        public string LoanApplicationId { get; set; }
    }
}