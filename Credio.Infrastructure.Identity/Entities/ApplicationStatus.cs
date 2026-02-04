using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class ApplicationStatus : BaseEntity
{
    public ApplicationStatus()
    {
        LoanApplications = new HashSet<LoanApplication>();
    }

    public ICollection<LoanApplication> LoanApplications { get; set; }
}