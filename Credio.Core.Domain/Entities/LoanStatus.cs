using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class LoanStatus : BaseEntity
{
    public LoanStatus()
    {
        Loans = [];
    }
    
    public bool IsActive { get; set; }
    
    public List<Loan> Loans { get; set; }
}