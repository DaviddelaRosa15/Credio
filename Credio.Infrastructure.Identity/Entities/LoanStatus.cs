using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class LoanStatus : BaseEntity
{
    public LoanStatus()
    {
        Loans = new HashSet<Loan>();
    }
    
    public bool IsActive { get; set; }

    public ICollection<Loan> Loans { get; set; }
}