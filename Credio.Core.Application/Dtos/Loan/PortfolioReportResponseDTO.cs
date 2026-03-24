using Microsoft.AspNetCore.Routing;

namespace Credio.Core.Application.Dtos.Loan;

public class PortfolioReportResponseDTO
{
    public PortfolioSummaryDto? Summary { get; set; } = new();

    public List<LoanReportItemDto> Data { get; set; } = [];
}

public class PortfolioSummaryDto
{
    public int TotalLoans { get; set; }
    
    public double TotalPortfolio { get; set; }
    
    public double LateFees { get; set; }
}

public class LoanReportItemDto
{
    public int LoanNumber { get; set; }

    public string Client { get; set; }

    public double OriginalAmount { get; set; }

    public double OutstandingBalance { get; set; }

    public int TotalFeePaidCount { get; set; }
    
    public int TotalFeeCount { get; set; }

    public int? DaysInArrears { get; set; }
    
    public string State { get; set; }
}