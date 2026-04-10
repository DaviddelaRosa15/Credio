using FluentValidation;

namespace Credio.Core.Application.Features.Loan.Queries.GetCollectorPortfolioQuery;

public class GetCollectorPortfolioQueryValidator : AbstractValidator<GetCollectorPortfolioQuery>
{
    private readonly string[] _estadosPermitidos = { "Todos", "Al Dia", "Vencidos" };
    
    public GetCollectorPortfolioQueryValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotNull().WithMessage("Employee ID no puede ser nulo")
            .NotEmpty().WithMessage("Employee ID no puede ser vacio");
        
        RuleFor(x => x.State)
            .Must(state => _estadosPermitidos.Contains(state))
            .When(x => !string.IsNullOrEmpty(x.State))
            .WithMessage("El campo State solo permite: Todos, Al Dia, Vencidos");
    }
}