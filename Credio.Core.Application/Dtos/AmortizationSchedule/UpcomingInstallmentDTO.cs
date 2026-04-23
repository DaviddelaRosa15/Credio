public class UpcomingInstallmentDTO
{
    public string Client { get; set; }

    public string DocumentNumber { get; set; }

    public int Loan { get; set; }

    public string LoanId { get; set; }

    public int InstallmentNumber { get; set; }

    public double DueAmount { get; set; }

    public decimal InterestAmount { get; set; }

    public decimal PrincipalAmount { get; set; }

    public decimal LateFeeAmount { get; set; }

    public DateOnly DueDate { get; set; }

    public string State { get; set; }
}