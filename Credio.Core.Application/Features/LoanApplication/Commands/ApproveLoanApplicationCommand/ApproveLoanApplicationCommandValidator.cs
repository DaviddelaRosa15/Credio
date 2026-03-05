using System.Security.Cryptography.Xml;
using FluentValidation;

namespace Credio.Core.Application.Features.LoanApplications.Commands.ApproveLoanApplicationCommand;

public class ApproveLoanApplicationCommandValidator : AbstractValidator<ApproveLoanApplicationCommand>
{
    public ApproveLoanApplicationCommandValidator()
    {
        RuleFor(x => x.LoanApplicationId)
            .NotNull().WithMessage("El id de la aplicacion del prestamo no puede estar nulo")
            .NotEmpty().WithMessage("El id de la aplication del prestamo no puede estar vacio");

        RuleFor(x => x.ApprovedAmount)
            .GreaterThan(0).WithMessage("El monto aprobado debe ser mayor a cero");
        
        RuleFor(x => x.ApprovedTerm)
            .GreaterThan(0).WithMessage("El termino aprobado debe ser mayor a cero");
        
        RuleFor(x => x.ApprovedInterestRate)
            .GreaterThan(0).WithMessage("La tasa aprobada debe ser mayor a cero");
    }
}