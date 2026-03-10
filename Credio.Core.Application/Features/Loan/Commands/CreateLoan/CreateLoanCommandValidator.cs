using FluentValidation;

namespace Credio.Core.Application.Features.LoanApplications.Commands.CreateLoan;

public class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
{
    public CreateLoanCommandValidator()
    {
        RuleFor(x => x.LoanApplicationId)
            .NotNull().WithMessage("La solicitud no puede estar nula")
            .NotEmpty().WithMessage("La solicitud no puede estar vacia");

        RuleFor(x => x.FirstPaymentDate)
            .NotNull().WithMessage("La primera fecha de pago no puede estar nula")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now));
    }
}