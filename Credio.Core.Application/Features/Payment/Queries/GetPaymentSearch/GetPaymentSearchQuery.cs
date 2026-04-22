using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;

namespace Credio.Core.Application.Features.Payment.Queries.GetPaymentSearch;

public class GetPaymentSearchQuery : IQuery<List<PaymentSearchDTO>>
{
    public string? SearchTerm { get; set; }
}

public class GetPaymentSearchQueryHandler : IQueryHandler<GetPaymentSearchQuery, List<PaymentSearchDTO>>
{
    private readonly ILoanRepository _loanRepository;

    public GetPaymentSearchQueryHandler(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }
    
    public async Task<Result<List<PaymentSearchDTO>>> Handle(GetPaymentSearchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _loanRepository.SearchLoansForPaymentAsync(request.SearchTerm, cancellationToken);
            
            return Result<List<PaymentSearchDTO>>.Success(response);
        }
        catch
        {
            return Result<List<PaymentSearchDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los pagos"));
        }
    }
}