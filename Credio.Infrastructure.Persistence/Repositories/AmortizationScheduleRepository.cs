using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class AmortizationScheduleRepository : GenericRepository<AmortizationSchedule>, IAmortizationScheduleRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContext;

        public AmortizationScheduleRepository(IDbContextFactory<ApplicationContext> dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PortfolioStateDTO> GetPortfolioState(CancellationToken cancellationToken)
        {
            using ApplicationContext db = _dbContext.CreateDbContext();
            
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            
            DateOnly next15Days = today.AddDays(15);

            return new PortfolioStateDTO
            {
                CurrentPercentage = await db.AmortizationSchedule
                    .CountAsync(x => x.AmortizationStatus.Description != "Pagada", cancellationToken),
                DueSoonPercentage = await db.AmortizationSchedule
                    .CountAsync(x => x.AmortizationStatus.Description != "Pagada" && x.DueDate < today,cancellationToken),
                OverduePercentage = await db.AmortizationSchedule
                    .CountAsync(x => x.AmortizationStatus.Description != "Pagada"
                                     && x.DueDate >= today
                                     && x.DueDate <= next15Days, cancellationToken)
            };
        }

        public async Task<List<decimal>> GetCollections(CancellationToken cancellationToken)
        {
            using ApplicationContext db = _dbContext.CreateDbContext();
            
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        
            DateOnly startDate = new DateOnly(today.Year, today.Month, 1).AddMonths(-6);
            
            var collections = await db.AmortizationSchedule
                .Where(x => x.DueDate >= startDate
                            && x.AmortizationStatus.Description == "Pagada")
                .GroupBy(x => new { x.DueDate.Year, x.DueDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Amount = g.Sum(x => x.PrincipalAmount)
                })
                .ToListAsync(cancellationToken);
            
            List<DateOnly> months = Enumerable.Range(0, 7)
                .Select(i => startDate.AddMonths(i))
                .ToList();
            
           return months
                .Select(m => collections
                    .Where(x => x.Year == m.Year && x.Month == m.Month)
                    .Select(x => x.Amount)
                    .FirstOrDefault())
                .ToList();
        }
    }
}
