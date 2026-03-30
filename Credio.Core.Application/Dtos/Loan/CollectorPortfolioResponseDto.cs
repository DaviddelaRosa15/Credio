namespace Credio.Core.Application.Dtos.Loan;

public class CollectorPortfolioResponseDto
{
    public CollectorPortfolioSummaryDto Summary { get; set; } 

    public List<CollectorPortfolioItemDto> Items { get; set; } 
}

public class CollectorPortfolioSummaryDto
{
    public int TotalCustomers { get; set; }

    public int PaymentsUpToDate { get; set; }

    public int OverduePayments { get; set; }

    public int OverDue { get; set; }
}

public class CollectorPortfolioItemDto
{
    public string CustomerName { get; set; } = string.Empty;

    public int LoanNumber { get; set; }

    public double Fee { get; set; }

    public DateOnly DueDate { get; set; }

    public string State { get; set; } = string.Empty;

    public int PaidInstallments { get; set; }

    public int TotalInstallments { get; set; }
}