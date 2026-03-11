using FluentValidation;

namespace Credio.Core.Application.Features.LoanApplications.Commands.RejectLoanApplicationCommand;

public class RejectLoanApplicationCommandValidator : AbstractValidator<RejectLoanApplicationCommand>
{
    public RejectLoanApplicationCommandValidator()
    {
        RuleFor(c => c.LoanApplicationId)
            .NotNull().WithMessage("El id de la aplicacion del prestamo no puede estar nulo")
            .NotEmpty().WithMessage("El id de la aplicacion del prestamo no puede estar vacio");
        
        RuleFor(c => c.RejectReason)
            .NotNull().WithMessage("La razon del rechazo no puede estar nulo")
            .NotEmpty().WithMessage("La razon del rechazo no puede estar vacio")
            .MinimumLength(10).WithMessage("La razon del rechazo como minimo debe de tener 10 caracteres");
    }
}