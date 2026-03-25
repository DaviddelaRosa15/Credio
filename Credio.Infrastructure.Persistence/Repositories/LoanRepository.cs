using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories;

public class LoanRepository : GenericRepository<Loan>, ILoanRepository
{
    private readonly IDbContextFactory<ApplicationContext> _dbContext;

    public LoanRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : base(dbContextFactory)
    {
        _dbContext = dbContextFactory;
    }

    public async Task<int> GetLastLoanNumberAsync()
    {
        using var db = _dbContext.CreateDbContext();

        // Obtener el n�mero de pr�stamos existentes en la base de datos
        int loanCount = db.Loan.Count();

        // Si no hay pr�stamos, el n�mero de pr�stamo ser� 0, de lo contrario, se obtiene el m�ximo n�mero de pr�stamo existente
        int lastNumber = loanCount != 0 ? db.Loan.Max(e => e.LoanNumber) : loanCount;

        return lastNumber;
    }

    public async Task<PortfolioSummaryDto?> GetPortfolioSummary(
        string? statusId, string? searchTerm, DateOnly? startDate, DateOnly? endDate,
        CancellationToken cancellationToken = default)
    {
        using ApplicationContext db = _dbContext.CreateDbContext();

        IQueryable<Loan> query = db.Loan
            .Where(predicate => 
                (string.IsNullOrEmpty(statusId) || predicate.LoanStatusId == statusId) &&
                (!startDate.HasValue || predicate.DisbursedDate >= startDate.Value) &&
                (!endDate.HasValue || predicate.EffectiveDate <= endDate.Value) &&
                (
                    string.IsNullOrEmpty(searchTerm) ||
                    predicate.Client.FirstName.Contains(searchTerm) ||
                    predicate.LoanNumber.ToString().Contains(searchTerm)
                ))
            .AsNoTracking();

        return new PortfolioSummaryDto
        {
            TotalLoans = await query.CountAsync(cancellationToken),
            LateFees = await query.SelectMany(x => x.LoanBalances).SumAsync(x => x.PrincipalBalance, cancellationToken),
            TotalPortfolio = await query.SelectMany(x => x.LateFees).SumAsync(x => x.Balance, cancellationToken),
        };
    }

    public async Task<List<double>> GetDisbursements(CancellationToken cancellationToken = default)
    {
        using ApplicationContext db = _dbContext.CreateDbContext();
        
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        DateOnly startDate = new DateOnly(today.Year, today.Month, 1).AddMonths(-6);
        
        var disbursements = await db.Loan
            .Where(x => x.DisbursedDate >= startDate)
            .GroupBy(x => new { x.DisbursedDate!.Value.Year, x.DisbursedDate.Value.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Amount = g.Sum(x => x.Amount)
            })
            .ToListAsync(cancellationToken);
        
        List<DateOnly> months = Enumerable.Range(0, 7)
            .Select(i => startDate.AddMonths(i))
            .ToList();
        
        return months
            .Select(m => disbursements
                .Where(x => x.Year == m.Year && x.Month == m.Month)
                .Select(x => x.Amount)
                .FirstOrDefault())
            .ToList();
    }

    public async Task<(int, double, double)> GetBasicDashboardMetrics(CancellationToken cancellationToken = default)
    {
        using ApplicationContext db = _dbContext.CreateDbContext();

        IQueryable<Loan> query = db.Loan
            .Where(x => x.LoanStatus.Description == "Activo")
            .AsNoTracking();
        
        // Basic Kpis
        int activeLoans = await query.CountAsync(cancellationToken);

        double totalPortfolio =
            await query.SelectMany(x => x.LoanBalances).SumAsync(x => x.PrincipalBalance, cancellationToken);

        double totalDelinquency = await query
            .SelectMany(x => x.LateFees)
            .Where(x => x.Balance > 0 || x.LateFeeStatus.Description != "Pagado")
            .SumAsync(x => x.Balance, cancellationToken);
        
        return (activeLoans, totalPortfolio, totalDelinquency);
    }
}