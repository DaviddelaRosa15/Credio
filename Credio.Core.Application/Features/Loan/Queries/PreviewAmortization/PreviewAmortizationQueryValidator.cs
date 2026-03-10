using FluentValidation;

namespace Credio.Core.Application.Features.Loan.Queries.PreviewAmortization;

public class PreviewAmortizationQueryValidator : AbstractValidator<PreviewAmortizationQuery>
{
    public PreviewAmortizationQueryValidator()
    {
        RuleFor(x => x.LoanApplicationId)
            .NotNull().WithMessage("El id de la solicitud no puede estar nulo")
            .NotEmpty().WithMessage("El id de la solicitud no puede estar vacio");
        
        RuleFor(x => x.FirstPaymentDate)
            .NotNull().WithMessage("La primera fecha de pago no puede estar nulo")
            .NotEmpty().WithMessage("La primera fecha de pago no puede estar vacio");
    }
}