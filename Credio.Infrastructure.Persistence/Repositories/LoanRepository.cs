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
            .Include(x => x.LoanBalance)
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
            TotalPortfolio = await query.SumAsync(
                x => x.LoanBalance != null ? x.LoanBalance.PrincipalBalance : 0, cancellationToken),
            LateFees = await query.SelectMany(x => x.LateFees).SumAsync(x => x.Balance, cancellationToken),
        };
    }

    public async Task<List<CashFlowItemDto>> GetDisbursements(CancellationToken cancellationToken = default)
    {
        using ApplicationContext db = _dbContext.CreateDbContext();

        // Getting today date in the date only format
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        // The start date is going to be the first day of the month, six months ago
        DateOnly startDate = new DateOnly(today.Year, today.Month, 1).AddMonths(-6);

        // Only loans disbursed within the last 7 months.
        var disbursements = await db.Loan
            .Where(x => x.DisbursedDate >= startDate)
            // Grouping by the disturbed date (year and month)
            .GroupBy(x => new { x.DisbursedDate!.Value.Year, x.DisbursedDate.Value.Month })
            .Select(g => new
            {
                g.Key.Year, //  { Year: 2025, Month: 10, Amount: 10000 },
                g.Key.Month,
                Amount = g.Sum(x => x.Amount)
            })
            .ToListAsync(cancellationToken);

        // Generating 7 months
        List<DateOnly> months = Enumerable.Range(0, 7)
            .Select(i => startDate.AddMonths(i))
            .ToList();

        // Building the array base in the months 
        return months
            .Select(m => new CashFlowItemDto
            {
                Month = $"{m:MMMM}",
                Year = $"{m:yyyy}",
                Amount = disbursements
                    .Where(x => x.Year == m.Year && x.Month == m.Month)
                    .Select(x => x.Amount)
                    .FirstOrDefault()
            })
            .ToList();
    }

    public async Task<(int, double, double)> GetBasicDashboardMetrics(CancellationToken cancellationToken = default)
    {
        using ApplicationContext db = _dbContext.CreateDbContext();

        IQueryable<Loan> query = db.Loan
            .Where(x => x.LoanStatus.Description == "Activo" || x.LoanStatus.Description == "En mora")
            .AsNoTracking();

        // Basic Kpis
        int activeLoans = await query.CountAsync(cancellationToken);

        double totalPortfolio =
            await query.SumAsync(x => x.LoanBalance != null ? x.LoanBalance.PrincipalBalance : 0, cancellationToken);

        double totalDelinquency = await query
            .SelectMany(x => x.LateFees)
            .Where(x => x.Balance > 0 || x.LateFeeStatus.Description != "Pagada")
            .SumAsync(x => x.Balance, cancellationToken);

        return (activeLoans, totalPortfolio, totalDelinquency);
    }

    public async Task<List<Loan>> GetActiveLoansByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        using ApplicationContext db = _dbContext.CreateDbContext();

        return await db.Loan
            .Include(x => x.LoanBalance)
            .Include(x => x.AmortizationSchedules)
                .ThenInclude(x => x.AmortizationStatus)
            .Where(x => (x.LoanStatus.Description == "Activo" || x.LoanStatus.Description == "En mora") && x.Client.DocumentNumber.Contains(documentNumber))
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<CollectorPortfolioResponseDto> GetCollectorPortfolio(
        string employeeId,
        string? searchTerm,
        string? state,
        CancellationToken cancellationToken = default)
    {
        using ApplicationContext db = _dbContext.CreateDbContext();

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        IQueryable<Loan> query = db.Loan.Where(
            predicate => predicate.EmployeeId == employeeId &&
                         predicate.LoanStatus.Description == "Activo")
            .Include(x => x.AmortizationSchedules);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p =>
                p.Client.FirstName.Contains(searchTerm) ||
                p.LoanNumber.ToString().Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(state) && state != "Todos")
        {
            if (state == "Al Dia")
            {
                query = query.Where(loan => !loan.AmortizationSchedules.Any(s => s.AmortizationStatus.Description == "Atrasada" || s.DueDate < today));
            }
            else if (state == "Vencidos")
            {
                query = query.Where(loan => loan.AmortizationSchedules.Any(s => s.DueDate < today));
            }
        }

        CollectorPortfolioSummaryDto summary = await query
            .GroupBy(_ => 1)
            .Select(g => new CollectorPortfolioSummaryDto
            {
                TotalCustomers = g.Select(x => x.ClientId).Distinct().Count(),

                PaymentsUpToDate = g.Count(loan => !loan.AmortizationSchedules.Any(s => s.AmortizationStatus.Description == "Atrasada" || s.DueDate < today)),

                OverduePayments = g.Count(loan => loan.AmortizationSchedules.Any(s => s.DueDate < today && s.AmortizationStatus.Description != "Pagada")),

                ToBeCollectedToday = g.Count(loan => loan.AmortizationSchedules.Any(s => s.DueDate == today && s.AmortizationStatus.Description != "Pagada"))
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken) ?? new CollectorPortfolioSummaryDto();

        List<CollectorPortfolioItemDto> items = await query
            .Select(loan => new CollectorPortfolioItemDto
            {
                CustomerName = loan.Client.FirstName + " " + loan.Client.LastName,

                LoanNumber = loan.LoanNumber,

                Fee = loan.AmortizationSchedules
                    .Where(x => x.AmortizationStatus.Description != "Pagada")
                    .OrderBy(x => x.DueDate)
                    .Select(x => x.DueAmount)
                    .FirstOrDefault(),

                NextPaymentDate = loan.AmortizationSchedules
                    .Where(x => x.AmortizationStatus.Description != "Pagada")
                    .OrderBy(x => x.DueDate)
                    .Select(x => x.DueDate)
                    .FirstOrDefault(),

                State = loan.AmortizationSchedules.Any(s => s.DueDate < today && s.AmortizationStatus.Description != "Pagada")
                    ? "Mora"
                    : "Al Día",

                PaidInstallments = loan.AmortizationSchedules.Count(s => s.AmortizationStatus.Description == "Pagada"),

                TotalInstallments = loan.AmortizationSchedules.Count()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new CollectorPortfolioResponseDto
        {
            Summary = summary,
            Items = items,
        };
    }

    public async Task<List<ClientDashboardLoanDTO>> GetClientLoansOverviewByClientId(string clientId, CancellationToken cancellationToken = default)
    {
        using ApplicationContext context = _dbContext.CreateDbContext();
        
        return await context.Loan
            .Where(l => l.Client.Id == clientId && l.LoanStatus.Description == "Activo")
            .Select(l => new ClientDashboardLoanDTO
            {
                LoanNumber = l.LoanNumber,
                Amount = l.Amount,
                LoanStatus = l.LoanStatus.Name,
                DisbursedDate = l.DisbursedDate,
                OutstandingBalance = l.LoanBalance != null ? l.LoanBalance.TotalOutstanding : 0,
                NextPayment = l.AmortizationSchedules
                    .Where(s => s.AmortizationStatus.Description == "Pendiente")
                    .OrderBy(s => s.DueDate)
                    .Select(s => s.DueDate) 
                    .FirstOrDefault(),
               MonthlyFee = l.AmortizationSchedules
                   .Where(s => s.AmortizationStatus.Description == "Pendiente")
                   .OrderBy(s => s.DueDate)
                   .Select(s => s.DueAmount) 
                   .FirstOrDefault(),
               FeesPaid = l.AmortizationSchedules.Count(x => x.AmortizationStatus.Description == "Pagada"),
               TotalFees = l.AmortizationSchedules.Count,
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<(int, double, double)> GetClientLoanSummaryByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        using ApplicationContext context = _dbContext.CreateDbContext();

        var result = await context.Loan
            .Include(x => x.LoanBalance)
            .Where(x => x.Client.Id == clientId && x.LoanStatus.Description == "Activo")
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                ActiveLoans = g.Count(),
                TotalBorrowed = g.Sum(x => x.Amount),
                OutstandingBalance = g.Sum(x => x.LoanBalance != null ? x.LoanBalance.TotalOutstanding : 0)
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return result is null
            ? (0, 0, 0)
            : (result.ActiveLoans, result.TotalBorrowed, result.OutstandingBalance);
    }
}