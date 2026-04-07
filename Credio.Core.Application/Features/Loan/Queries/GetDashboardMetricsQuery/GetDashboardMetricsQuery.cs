using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;

namespace Credio.Core.Application.Features.Loan.Queries.GetDashboardMetricsQuery;

public class GetDashboardMetricsQuery : IQuery<DashboardMetricsDTO>;

public class GetDashboardMetricsQueryHandler : IQueryHandler<GetDashboardMetricsQuery, DashboardMetricsDTO>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAmortizationScheduleRepository _amortizationScheduleRepository;

    public GetDashboardMetricsQueryHandler(ILoanRepository loanRepository, IAmortizationScheduleRepository amortizationScheduleRepository)
    {
        _loanRepository = loanRepository;
        _amortizationScheduleRepository = amortizationScheduleRepository;
    }
    
    public async Task<Result<DashboardMetricsDTO>> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        try
        {
          (int activeLoans, double totalPortfolio, double totalDelinquency) = await _loanRepository.GetBasicDashboardMetrics(cancellationToken);
          
          List<double> disbursements = await _loanRepository.GetDisbursements(cancellationToken);
          
          List<decimal> collections = await _amortizationScheduleRepository.GetCollections(cancellationToken);

          PortfolioStateDTO portfolioState = await _amortizationScheduleRepository.GetPortfolioState(cancellationToken);

          return Result<DashboardMetricsDTO>.Success(new DashboardMetricsDTO
          {
              ActiveLoans = activeLoans,
              TotalPortfolio = totalPortfolio,
              TotalDelinquency = totalDelinquency,
              AvailableLiquidity = 0,
              CashFlow = new CashFlowDTO
              {
                  Collections = collections,
                  Disbursements = disbursements
              },
              PortfolioState =  portfolioState
          });
        }
        catch
        {
            return Result<DashboardMetricsDTO>.Failure(Error.InternalServerError("Ocurrio un error al obtener los kpis principales"));
        }
    }
}