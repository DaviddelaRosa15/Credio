namespace Credio.Core.Application.Dtos.Loan;

public class ClientDashboardResponseDTO
{
    public int ActiveLoans { get; set; }

    public double TotalBorrowed { get; set; }

    public double OutstandingBalance { get; set; }

    public OfficerInfoDTO? OfficerInfo { get; set; }

    public List<ClientDashboardLoanDTO> Loans { get; set; }
}

public class OfficerInfoDTO
{
    public string OfficerFirstName { get; set; }
    
    public string OfficerLastName { get; set; }
    
    public string? OfficerEmail { get; set; }
}

public class ClientDashboardLoanDTO
{
    public int LoanNumber { get; set; }

    public string LoanStatus { get; set; }

    public DateOnly? DisbursedDate { get; set; }

    public double Amount { get; set; }

    public double OutstandingBalance{ get; set; }

    public decimal MonthlyFee { get; set; }

    public int FeesPaid { get; set; }

    public DateOnly NextPayment { get; set; }

    public int TotalFees { get; set; }
}