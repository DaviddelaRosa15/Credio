using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Features.Loan.Queries.GetNextPaymentSummaryByDocumentQuery;

public class GetNextPaymentSummaryByDocumentQuery : IQuery<BotNextPaymentResponseDTO>
{
    public string DocumentNumber { get; set; } = null!;
}

public class GetNextPaymentSummaryByDocumentQueryHandler : IQueryHandler<GetNextPaymentSummaryByDocumentQuery, BotNextPaymentResponseDTO>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IMapper _mapper;

    public GetNextPaymentSummaryByDocumentQueryHandler(ILoanRepository loanRepository, IMapper mapper)
    {
        _loanRepository = loanRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<BotNextPaymentResponseDTO>> Handle(GetNextPaymentSummaryByDocumentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<Domain.Entities.Loan> activeLoans =
                await _loanRepository.GetActiveLoansByDocumentNumberAsync(request.DocumentNumber, cancellationToken);

            if (activeLoans.Count <= 0)
            {
                return Result<BotNextPaymentResponseDTO>.Failure(Error.NotFound("No se ha encontrado prestamo activo al numero de documento dado"));
            }

            // Get the next outstanding payment for each loan
            var outstandingPayment = activeLoans.Select(loan => 
                {
                    AmortizationSchedule? nextPayment = loan.AmortizationSchedules
                        .Where(x => x.AmortizationStatus.Description == "Pendiente")
                        .OrderBy(x => x.DueDate)
                        .FirstOrDefault();
            
                    double lateFee = loan.LoanBalance.LateFeeBalance; 

                    return new
                    {
                        Loan = loan,
                        NextPayment = nextPayment,
                        LateFee = lateFee,
                        TotalAmountToPay = (double)(nextPayment?.PrincipalAmount ?? 0) + lateFee
                    };
                })
                .Where(x => x.NextPayment is not null)
                .OrderBy(x => x.NextPayment!.DueDate)
                .ToList();
            
            List<BotPaymentDetailDTO> mapped = outstandingPayment
                .Select(x => new BotPaymentDetailDTO
                {
                    LoanNumber = x.Loan.LoanNumber,
                    DaysUntilDue = x.Loan.LoanBalance.DaysInArrears,
                    InstallmentAmount = x.NextPayment!.InstallmentNumber,
                    DueTime = x.NextPayment.DueDate,
                    LateFeeAmount = x.LateFee,
                    TotalAmountToPay = x.TotalAmountToPay
                })
                .ToList();

            return Result<BotNextPaymentResponseDTO>.Success(new BotNextPaymentResponseDTO
            {
                UrgentPayment = mapped.FirstOrDefault(),
                OtherActiveLoans = mapped.Skip(1).ToList()
            });
        }
        catch
        {
            return Result<BotNextPaymentResponseDTO>.Failure(Error.InternalServerError("Hubo un error al obtener las cuotas proximas"));
        }
    }
}
