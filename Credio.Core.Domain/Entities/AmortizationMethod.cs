using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class AmortizationMethod : BaseEntity
{
    public AmortizationMethod()
    {
        Loans = [];
    }
    
    public List<Loan> Loans { get; set; }
}