using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Features.Loan.Queries.GetClientDashboardQuery;

public class GetClientDashboardQuery : IQuery<ClientDashboardResponseDTO>
{
    public string ClientId { get; set; }
}

public class GetClientDashboardQueryHandler : IQueryHandler<GetClientDashboardQuery, ClientDashboardResponseDTO>
{
    private readonly IClientRepository _clientRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetClientDashboardQueryHandler(
        IClientRepository  clientRepository,
        ILoanRepository  loanRepository, 
        ICurrentUserService currentUserService)
    {
        _clientRepository = clientRepository;
        _loanRepository = loanRepository;
        _currentUserService = currentUserService;
    }
    public async Task<Result<ClientDashboardResponseDTO>> Handle(GetClientDashboardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            string? currentClientId = _currentUserService.GetCurrentUserId();

            if (string.IsNullOrEmpty(currentClientId) || string.IsNullOrWhiteSpace(currentClientId))
            {
                return Result<ClientDashboardResponseDTO>.Failure(Error.BadRequest("Sesión no válida o expirada"));
            }
            
            if (!_currentUserService.isInRole("Administrator") && currentClientId != request.ClientId)
            {
                return Result<ClientDashboardResponseDTO>.Failure(Error.BadRequest("No tienes permiso para ver el dashboard de otro cliente"));
            }
            
            (int activeLoans, double totalBorrowed, double outstandingBalance) = await _loanRepository.GetClientLoanSummaryByClientIdAsync(request.ClientId, cancellationToken);

            OfficerInfoDTO? officeInfo = await _clientRepository.GetOfficerInfoByClientId(request.ClientId, cancellationToken);
            
            List<ClientDashboardLoanDTO> clientLoansOverview = await _loanRepository.GetClientLoansOverviewByClientId(request.ClientId, cancellationToken);
            
            return Result<ClientDashboardResponseDTO>.Success(new ClientDashboardResponseDTO
            {
                ActiveLoans = activeLoans,
                TotalBorrowed = totalBorrowed,
                OutstandingBalance = outstandingBalance,
                OfficerInfo = officeInfo,
                Loans =  clientLoansOverview
            });
        }
        catch 
        {
            return Result<ClientDashboardResponseDTO>.Failure(Error.InternalServerError("Hubo un error al intentar consultar la informacion del cliente"));
        }
    }
}