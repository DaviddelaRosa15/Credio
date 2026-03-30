using FluentValidation;

namespace Credio.Core.Application.Features.Loan.Queries.GetLoanAmortizationSchedule;

public class GetLoanAmortizationScheduleQueryValidator : AbstractValidator<GetLoanAmortizationScheduleQuery>
{
    public GetLoanAmortizationScheduleQueryValidator()
    {
        RuleFor(x => x.LoanId)
            .NotNull().WithMessage("El id del préstamo no puede estar nulo")
            .NotEmpty().WithMessage("El id del préstamo no puede estar vacio");
    }
}