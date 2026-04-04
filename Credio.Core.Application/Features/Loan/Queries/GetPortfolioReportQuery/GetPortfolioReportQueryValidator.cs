using FluentValidation;

namespace Credio.Core.Application.Features.Loan.Queries.GetPortfolioReportQuery;

public class GetPortfolioReportQueryValidator : AbstractValidator<GetPortfolioReportQuery>
{
    public GetPortfolioReportQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("La fecha de inicio no puede ser mayor a la fecha final");
    }
}