using Credio.Core.Application.Features.LoanApplications.Commands.RejectLoanApplicationCommand;

namespace Credio.Core.Application.Dtos.Requests;

public class RejectLoanApplicationRequest
{
    public string RejectReason { get; set; } = string.Empty;

    public RejectLoanApplicationCommand ToCommand(string loanApplicationId)
    {
        return new RejectLoanApplicationCommand
        {
            LoanApplicationId = loanApplicationId,
            RejectReason = RejectReason
        };
    }
}