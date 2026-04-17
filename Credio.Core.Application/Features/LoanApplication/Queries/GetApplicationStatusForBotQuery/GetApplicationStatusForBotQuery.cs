using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;

namespace Credio.Core.Application.Features.LoanApplication.Queries.GetApplicationStatusForBotQuery;

public class GetApplicationStatusForBotQuery : IQuery<List<BotApplicationStatusDTO>>
{
    public string DocumentNumber { get; set; }
}

public class GetApplicationStatusForBotQueryHandler : IQueryHandler<GetApplicationStatusForBotQuery, List<BotApplicationStatusDTO>>
{
    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly IMapper _mapper;

    public GetApplicationStatusForBotQueryHandler(
        ILoanApplicationRepository loanApplicationRepository,
        IMapper mapper)
    {
        _loanApplicationRepository = loanApplicationRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<List<BotApplicationStatusDTO>>> Handle(GetApplicationStatusForBotQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<Domain.Entities.LoanApplication> applications = await _loanApplicationRepository.GetAllByPropertyWithIncludeAsync(
                x => x.Client.DocumentNumber == request.DocumentNumber, [x => x.ApplicationStatus]);

            List<BotApplicationStatusDTO> result = _mapper.Map<List<BotApplicationStatusDTO>>(applications.OrderByDescending(x => x.Created).ToList());
            
            return Result<List<BotApplicationStatusDTO>>.Success(result);
        }
        catch
        {
            return Result<List<BotApplicationStatusDTO>>.Failure(Error.InternalServerError("Ocurrio un error al momento de obtener las solicitudes del usuario"));
        }
    }
}