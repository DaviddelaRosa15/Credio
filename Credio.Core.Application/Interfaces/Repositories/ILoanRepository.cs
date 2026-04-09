using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Repositories;

public interface ILoanRepository : IGenericRepository<Loan> 
{
    Task<int> GetLastLoanNumberAsync();

    Task<PortfolioSummaryDto?> GetPortfolioSummary(
        string? statusId,
        string? searchTerm,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken = default);
    
    Task<(int, double, double)> GetBasicDashboardMetrics(CancellationToken cancellationToken = default);

    Task<List<double>> GetDisbursements(CancellationToken cancellationToken = default);
    
    Task<List<Loan>> GetActiveLoansByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    
    Task<CollectorPortfolioResponseDto>  GetCollectorPortfolio(
        string employeeId,
        string? searchTerm,
        string? state,
        CancellationToken cancellationToken = default);
}