using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Credio.Core.Application.Features.Loan.Queries.GetDisbursementReceiptPdf
{
    public class GetDisbursementReceiptPdfQuery : IQuery<DisburseLoanNotificationDTO>
    {
        public string LoanId { get; set; } = string.Empty;
    }

    public class GetDisbursementReceiptPdfQueryHandler : IQueryHandler<GetDisbursementReceiptPdfQuery, DisburseLoanNotificationDTO>
    {
        private readonly ILoanRepository _loanRepository;

        public GetDisbursementReceiptPdfQueryHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Result<DisburseLoanNotificationDTO>> Handle(GetDisbursementReceiptPdfQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Consultamos el préstamo con su cliente y frecuencia de pago para obtener la información necesaria para el recibo de desembolso
                var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == query.LoanId,
                    l => l.Include(l => l.Client).Include(l => l.PaymentFrequency));

                // Si no se encuentra el préstamo, retornamos un error de not found
                if (loan is null) return Result<DisburseLoanNotificationDTO>.Failure(Error.NotFound("No se encontro el préstamo con el id dado"));

                if (loan.DisbursedDate is null) return Result<DisburseLoanNotificationDTO>.Failure(Error.BadRequest("El préstamo no ha sido desembolsado aún"));

                // Construimos el DTO con la información del préstamo y su cliente para el recibo de desembolso
                var result = new DisburseLoanNotificationDTO
                {
                    ClientName = $"{loan.Client.FirstName} {loan.Client.LastName}",
                    DocumentNumber = loan.Client.DocumentNumber,
                    EffectiveDate = (DateOnly)loan.DisbursedDate,
                    InterestRate = loan.InterestRate,
                    LoanAmount = loan.Amount,
                    LoanId = loan.Id,
                    LoanNumber = loan.LoanNumber,
                    LoanStatus = "Activo",
                    PaymentFrequency = loan.PaymentFrequency.Name,
                    Term = loan.Term
                };

                return Result<DisburseLoanNotificationDTO>.Success(result);
            }
            catch
            {
                return Result<DisburseLoanNotificationDTO>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando el préstamo"));
            }
        }
    }
}