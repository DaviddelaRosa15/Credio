using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Interfaces.Repositories
{
    public interface IAmortizationScheduleRepository : IGenericRepository<AmortizationSchedule>
    {
        Task<PortfolioStateDTO> GetPortfolioState(CancellationToken cancellationToken);

        Task<List<CashFlowItemDto>> GetCollections(CancellationToken cancellationToken);
    }
}
