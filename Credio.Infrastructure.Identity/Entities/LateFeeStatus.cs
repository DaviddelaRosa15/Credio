using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class LateFeeStatus : BaseEntity
{
    public LateFeeStatus()
    {
        LateFees = new HashSet<LateFee>();
    }
    
    public ICollection<LateFee> LateFees { get; set; }
}