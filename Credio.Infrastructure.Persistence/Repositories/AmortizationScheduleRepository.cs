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
            
            // By default, is a truncated date (DateOnly)
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            
            DateOnly next7Days = today.AddDays(7);
            
            List<AmortizationSchedule> installments  = await db.AmortizationSchedule
                .Include(x => x.AmortizationStatus)
                .ToListAsync(cancellationToken);

            return new PortfolioStateDTO
            {
                // All installments paid (regardless of their date) and Those that are pending,
                // but their due date is in the future and do NOT fall within the next 7 days
                CurrentPercentage = installments
                    .Count(x => x.AmortizationStatus.Description == "Pagada" || (x.AmortizationStatus.Description != "Pagada" && x.DueDate > next7Days)),
                // The due date (dueDate) has already passed AND they are not paid.
                OverduePercentage = installments
                    .Count(x => x.AmortizationStatus.Description != "Pagada" && x.DueDate < today),
                // The due date (dueDate) is in the range of Today to 7 days in the future AND they are not paid.
                DueSoonPercentage = installments
                    .Count(x => x.AmortizationStatus.Description != "Pagada" && x.DueDate >= today && x.DueDate <= next7Days),
                // Total number of installments that were extracted from the database
                TotalInstallments = installments.Count,
            };
        }

        public async Task<List<CashFlowItemDto>> GetCollections(CancellationToken cancellationToken)
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
                .Select(m => new CashFlowItemDto
                {
                    Month = $"{m:MMMM}",
                    Year = $"{m:yyyy}",
                    Amount = (double)collections
                        .Where(x => x.Year == m.Year && x.Month == m.Month)
                        .Select(x => x.Amount)
                        .FirstOrDefault()
                })
                .ToList();
        }
    }
}
