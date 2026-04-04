using FluentValidation;

namespace Credio.Core.Application.Features.LoanApplication.Queries.SimulateAmortization;

public class SimulateAmortizationQueryValidator : AbstractValidator<SimulateAmortizationQuery>
{
    public SimulateAmortizationQueryValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto del préstamo debe ser mayor a cero.");

        RuleFor(x => x.InterestRate)
            .GreaterThan(0).WithMessage("La tasa de interés debe ser mayor a cero.")
            .LessThanOrEqualTo(100).WithMessage("La tasa de interés supera el límite permitido.");

        RuleFor(x => x.PaymentFrequencyId)
            .NotNull().WithMessage("Debe especificar la frecuencia de pago.")
            .NotEmpty().WithMessage("Debe especificar la frecuencia de pago.");

        RuleFor(x => x.Term)
            .GreaterThan(0).WithMessage("El plazo del préstamo debe ser mayor a cero.");
    }
}