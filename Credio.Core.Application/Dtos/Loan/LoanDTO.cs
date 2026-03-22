namespace Credio.Core.Application.Dtos.Loan;

public class LoanDTO
{
    public string Id { get; set; }

    public int LoanNumber { get; set; }

    public string ClientId { get; set; }

    public string ClientName { get; set; }
    
    public string EmployeeId { get; set; }

    public string LoanStatus { get; set; }
    
    public double Amount { get; set; }

    public double InterestRate { get; set; }

    public int Term { get; set; }

    public string Frequency { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public DateOnly FirstPaymentDate { get; set; }

    public DateOnly MaturityDate { get; set; }
}