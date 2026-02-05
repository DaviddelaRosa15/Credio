using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class ApplicationStatus : BaseEntity
{
    public ApplicationStatus()
    {
        LoanApplications = [];
    }
    
    public List<LoanApplication> LoanApplications { get; set; }
}