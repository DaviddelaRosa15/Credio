using FluentValidation;

namespace Credio.Core.Application.Features.Loan.Queries.GetClientDashboardQuery;

public class GetClientDashboardQueryValidator : AbstractValidator<GetClientDashboardQuery>
{
    public GetClientDashboardQueryValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ClientId no puede estar vacio")
            .NotNull().WithMessage("ClientId no puede estar nulo");
    }
}