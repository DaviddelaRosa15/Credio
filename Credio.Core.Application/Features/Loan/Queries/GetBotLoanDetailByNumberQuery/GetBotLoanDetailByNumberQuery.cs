using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;

namespace Credio.Core.Application.Features.Loan.Queries.GetBotLoanDetailByNumberQuery;

public class GetBotLoanDetailByNumberQuery : IQuery<BotLoanDetailDTO>
{
    public int LoanNumber { get; set; }
}

public class GetBotLoanDetailByNumberQueryHandler : IQueryHandler<GetBotLoanDetailByNumberQuery, BotLoanDetailDTO>
{
    private readonly ILoanRepository _loanRepository;

    public GetBotLoanDetailByNumberQueryHandler(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }
    
    public async Task<Result<BotLoanDetailDTO>> Handle(GetBotLoanDetailByNumberQuery request, CancellationToken cancellationToken)
    {
        try
        {
            BotLoanDetailDTO? response = await _loanRepository.GetBotLoanDetailByLoanNumber(request.LoanNumber, cancellationToken);

            if (response is null)
            {
                return Result<BotLoanDetailDTO>.Failure(Error.NotFound("No se encontro informacion con relacion al numero de prestamo dado"));
            }

            return Result<BotLoanDetailDTO>.Success(response);
        }
        catch 
        {
            return Result<BotLoanDetailDTO>.Failure(Error.InternalServerError("Hubo un error al intentar consultar la informacion del prestamo"));
        }
    }
}