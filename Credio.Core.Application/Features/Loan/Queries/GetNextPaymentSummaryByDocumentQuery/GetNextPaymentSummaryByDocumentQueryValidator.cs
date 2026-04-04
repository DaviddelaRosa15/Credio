using FluentValidation;

namespace Credio.Core.Application.Features.Loan.Queries.GetNextPaymentSummaryByDocumentQuery;

public class GetNextPaymentSummaryByDocumentQueryValidator : AbstractValidator<GetNextPaymentSummaryByDocumentQuery>
{
    public GetNextPaymentSummaryByDocumentQueryValidator()
    {
        RuleFor(x => x.DocumentNumber)
            .NotNull().WithMessage("El numero de documento es requerido")
            .NotEmpty().WithMessage("La numero de documento es requerido");
    }
}