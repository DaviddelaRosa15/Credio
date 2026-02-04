using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class AmortizationMethod : BaseEntity
{
    public AmortizationMethod()
    {
        Loans = new HashSet<Loan>();    
    }
    
    public ICollection<Loan> Loans { get; set; }
}