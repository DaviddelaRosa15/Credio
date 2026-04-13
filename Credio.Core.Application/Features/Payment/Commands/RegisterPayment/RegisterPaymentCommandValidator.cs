using FluentValidation;

namespace Credio.Core.Application.Features.Payment.Commands.RegisterPayment
{
    public class RegisterPaymentCommandValidator : AbstractValidator<RegisterPaymentCommand>
    {
        public RegisterPaymentCommandValidator()
        {
            RuleFor(x => x.LoanId)
                .NotEmpty().WithMessage("El ID del préstamo es requerido.");

            RuleFor(x => x.CollectorCode)
                .NotEmpty().WithMessage("El código del cobrador es requerido.");

            RuleFor(x => x.PaymentMethodId)
                .NotEmpty().WithMessage("El método de pago es requerido.");

            RuleFor(x => x.AmountPaid)
                .GreaterThan(0).WithMessage("El monto a pagar debe ser una cifra positiva.");

            RuleFor(x => x.GpsLatitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.GpsLatitude.HasValue)
                .WithMessage("La latitud debe estar en el rango de -90 a 90 grados.");

            RuleFor(x => x.GpsLongitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.GpsLongitude.HasValue)
                .WithMessage("La longitud debe estar en el rango de -180 a 180 grados.");
        }
    }
}
