namespace Credio.Core.Application.Dtos.Loan;

public class BotLoanDetailDTO
{
    public int LoanNumber { get; set; }

    public string StatusName { get; set; }

    public double OriginalAmount { get; set; }

    public double CurrentBalance { get; set; }

    public DateOnly NextPaymentDate { get; set; }

    public double NextPaymentAmount { get; set; }
}