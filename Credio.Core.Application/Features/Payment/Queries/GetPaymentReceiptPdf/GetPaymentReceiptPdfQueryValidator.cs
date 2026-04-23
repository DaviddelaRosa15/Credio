using FluentValidation;

namespace Credio.Core.Application.Features.Payment.Queries.GetPaymentReceiptPdf;

public class GetPaymentReceiptPdfQueryValidator : AbstractValidator<GetPaymentReceiptPdfQuery>
{
    public GetPaymentReceiptPdfQueryValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotNull().WithMessage("El id del pago no puede estar nulo")
            .NotEmpty().WithMessage("El id del pago no puede estar vacio");
    }
}