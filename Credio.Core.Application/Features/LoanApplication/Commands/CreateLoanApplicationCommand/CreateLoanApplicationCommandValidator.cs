using FluentValidation;

namespace Credio.Core.Application.Features.LoanApplications.Commands.CreateLoanApplicationCommand;

public class CreateLoanApplicationCommandValidator : AbstractValidator<CreateLoanApplicationCommand>
{
    public CreateLoanApplicationCommandValidator()
    {
        RuleFor(x => x.RequestedAmount)
            .GreaterThan(0).WithMessage("La cantidad solicitada debe ser mayor a 0");
        
        RuleFor(x => x.RequestedInterestRate)
            .GreaterThan(0).WithMessage("La tasa de interes solicitada debe ser mayor a 0");

        RuleFor(x => x.RequestedTerm)
            .GreaterThan(0).WithMessage("El termino solicitado debe ser mayor a 0");

        RuleFor(x => x.ClientId)
            .NotNull().WithMessage("El id del cliente no puede estar nulo")
            .NotEmpty().WithMessage("La id del cliente no puede estar vacio");
        
        RuleFor(x => x.EmployeeId)
            .NotNull().WithMessage("El id del empleado no puede estar nulo")
            .NotEmpty().WithMessage("La id del empleado no puede estar vacio");
    }
}