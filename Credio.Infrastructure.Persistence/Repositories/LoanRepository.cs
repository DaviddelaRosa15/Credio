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

        // Obtener el número de préstamos existentes en la base de datos
        int loanCount = db.Loan.Count();

        // Si no hay préstamos, el número de préstamo será 0, de lo contrario, se obtiene el máximo número de préstamo existente
        int lastNumber = loanCount != 0 ? db.Loan.Max(e => e.LoanNumber) : loanCount;

        return lastNumber;
    }
}