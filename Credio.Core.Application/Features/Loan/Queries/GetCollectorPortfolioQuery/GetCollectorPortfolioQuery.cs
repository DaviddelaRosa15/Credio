using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Features.Loan.Queries.GetCollectorPortfolioQuery;

public class GetCollectorPortfolioQuery : IQuery<CollectorPortfolioResponseDto>
{
    public string EmployeeId { get; set; }
    
    public string? SearchTerm { get; set; }

    public string? State { get; set; }
}

public class GetCollectorPortfolioQueryHandler : IQueryHandler<GetCollectorPortfolioQuery,CollectorPortfolioResponseDto >
{
    private readonly ILoanRepository _loanRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCollectorPortfolioQueryHandler(ILoanRepository loanRepository, ICurrentUserService currentUserService)
    {
        _loanRepository = loanRepository;
        _currentUserService = currentUserService;
    }
    
    public async Task<Result<CollectorPortfolioResponseDto>> Handle(GetCollectorPortfolioQuery request, CancellationToken cancellationToken)
    {
        try
        {
            string? currentEmployeeId = _currentUserService.GetCurrentUserId();

            if (string.IsNullOrEmpty(currentEmployeeId))
            {
                return Result<CollectorPortfolioResponseDto>.Failure(Error.BadRequest("Sesión no válida o expirada"));
            }

            if (!_currentUserService.isInRole("Administrator") && currentEmployeeId != request.EmployeeId)
            {
                return Result<CollectorPortfolioResponseDto>.Failure(Error.BadRequest("No tienes permiso para ver la cartera de otro empleado"));
            }

            CollectorPortfolioResponseDto response = await _loanRepository.GetCollectorPortfolio(request.EmployeeId,
                request.SearchTerm,
                request.State,
                cancellationToken);
            
            return Result<CollectorPortfolioResponseDto>.Success(response);
        }
        catch
        {
            return Result<CollectorPortfolioResponseDto>.Failure(Error.InternalServerError("Hubo un error al intentar consultar la informacion del cobrador y sus clientes"));
        }
    }
}