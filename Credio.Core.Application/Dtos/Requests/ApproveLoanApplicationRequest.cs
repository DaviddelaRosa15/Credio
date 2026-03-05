using Credio.Core.Application.Features.LoanApplications.Commands.ApproveLoanApplicationCommand;

namespace Credio.Core.Application.Dtos.Requests;

public class ApproveLoanApplicationRequest
{
    public double ApprovedAmount { get; set; }
    
    public int ApprovedTerm { get; set; }
    
    public double ApprovedInterestRate { get; set; }

    public ApproveLoanApplicationCommand ToCommand(string loanApplicationId)
    {
        return new ApproveLoanApplicationCommand
        {
            LoanApplicationId = loanApplicationId,
            ApprovedAmount = ApprovedAmount,
            ApprovedTerm = ApprovedTerm,
            ApprovedInterestRate = ApprovedInterestRate
        };
    }
}