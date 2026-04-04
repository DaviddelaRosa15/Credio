namespace Credio.Core.Application.Dtos.Loan;

public class DashboardMetricsDTO
{
    public double TotalPortfolio { get; set; }

    public double AvailableLiquidity { get; set; }

    public double TotalDelinquency { get; set; }

    public int ActiveLoans { get; set; }

    public CashFlowDTO CashFlow { get; set; }

    public PortfolioStateDTO PortfolioState { get; set; }
}

public class CashFlowDTO
{
    public List<double> Disbursements { get; set; } = new();
    public List<decimal> Collections { get; set; } = new();
}

public class PortfolioStateDTO
{
    public double CurrentPercentage { get; set; }   
    public double OverduePercentage { get; set; }     
    public double DueSoonPercentage { get; set; }     
}