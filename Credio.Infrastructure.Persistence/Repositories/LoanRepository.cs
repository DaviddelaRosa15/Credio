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
        string statusId, string searchTerm, DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        using ApplicationContext db = _dbContext.CreateDbContext();

        IQueryable<Loan> query = db.Loan
            .Where(x =>
                x.LoanStatusId == statusId &&
                x.DisbursedDate >= startDate &&
                x.EffectiveDate <= endDate &&
                x.Client.FirstName.Contains(searchTerm) ||
                x.LoanNumber.ToString().Contains(searchTerm))
            .AsNoTracking();

        return new PortfolioSummaryDto
        {
            TotalLoans = await query.CountAsync(cancellationToken),
            LateFees = await query.SelectMany(x => x.LoanBalances).SumAsync(x => x.PrincipalBalance, cancellationToken),
            TotalPortfolio = await query.SelectMany(x => x.LateFees).SumAsync(x => x.Balance, cancellationToken),
        };
    }
}