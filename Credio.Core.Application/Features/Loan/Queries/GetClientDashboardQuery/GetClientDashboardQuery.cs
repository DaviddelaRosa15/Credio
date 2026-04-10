using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;

namespace Credio.Core.Application.Features.Loan.Queries.GetClientDashboardQuery;

public class GetClientDashboardQuery : IQuery<ClientDashboardResponseDTO>
{
    public string ClientId { get; set; }
}

public class GetClientDashboardQueryHandler : IQueryHandler<GetClientDashboardQuery, ClientDashboardResponseDTO>
{
    private readonly IClientRepository _clientRepository;
    private readonly ILoanRepository _loanRepository;

    public GetClientDashboardQueryHandler(
        IClientRepository  clientRepository,
        ILoanRepository  loanRepository)
    {
        _clientRepository = clientRepository;
        _loanRepository = loanRepository;
    }
    public async Task<Result<ClientDashboardResponseDTO>> Handle(GetClientDashboardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            OfficerInfoDTO? officeInfo = await _clientRepository.GetOfficerInfoByClientId(request.ClientId, cancellationToken);
            
            ClientDashboardResponseDTO clientDashBoard = await _loanRepository.GetClientDashboard(request.ClientId, cancellationToken);
            
            clientDashBoard.OfficerInfo = officeInfo;
            
            return Result<ClientDashboardResponseDTO>.Success(clientDashBoard);
        }
        catch 
        {
            return Result<ClientDashboardResponseDTO>.Failure(Error.InternalServerError("Hubo un error al intentar consultar la informacion del cliente"));
        }
    }
}