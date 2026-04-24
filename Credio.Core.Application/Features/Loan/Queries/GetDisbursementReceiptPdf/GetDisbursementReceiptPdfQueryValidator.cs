using FluentValidation;

namespace Credio.Core.Application.Features.Loan.Queries.GetDisbursementReceiptPdf;

public class GetDisbursementReceiptPdfQueryValidator : AbstractValidator<GetDisbursementReceiptPdfQuery>
{
    public GetDisbursementReceiptPdfQueryValidator()
    {
        RuleFor(x => x.LoanId)
            .NotNull().WithMessage("El id del préstamo no puede estar nulo")
            .NotEmpty().WithMessage("El id del préstamo no puede estar vacio");
    }
}