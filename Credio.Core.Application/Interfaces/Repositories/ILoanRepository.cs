using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Repositories;

public interface ILoanRepository : IGenericRepository<Loan> 
{
    Task<int> GetLastLoanNumberAsync();

    Task<PortfolioSummaryDto?> GetPortfolioSummary(
        string? statusId, string? searchTerm, DateOnly? startDate, DateOnly? endDate,
        CancellationToken cancellationToken = default);
}