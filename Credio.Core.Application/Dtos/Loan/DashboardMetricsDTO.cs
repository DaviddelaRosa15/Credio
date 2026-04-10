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
    public List<CashFlowItemDto> Disbursements { get; set; } = new();
    public List<CashFlowItemDto> Collections { get; set; } = new();
}

public class CashFlowItemDto
{
    public string Month { get; set; }

    public string Year { get; set; }

    public double Amount { get; set; }
}

public class PortfolioStateDTO
{
    public double CurrentPercentage { get; set; }   
    public double OverduePercentage { get; set; }     
    public double DueSoonPercentage { get; set; }
    
    public int TotalInstallments { get; set; }
}