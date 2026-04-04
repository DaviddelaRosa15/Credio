using FluentValidation;

namespace Credio.Core.Application.Features.LoanApplications.Commands.DisburseLoan;

public class DisburseLoanCommandValidator : AbstractValidator<DisburseLoanCommand>
{
    public DisburseLoanCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .NotNull().WithMessage("El préstamo no puede estar nulo")
            .NotEmpty().WithMessage("El préstamo no puede estar vacío");

        RuleFor(x => x.EffectiveDate)
            .NotNull().WithMessage("La fecha no puede estar nula")
            .NotEmpty().WithMessage("La fecha no puede estar vacía");
    }
}