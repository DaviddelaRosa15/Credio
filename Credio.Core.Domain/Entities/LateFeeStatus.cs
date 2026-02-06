using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class LateFeeStatus : BaseEntity
{
    public LateFeeStatus()
    {
        LateFees = [];
    }
    
    public List<LateFee> LateFees { get; set; }
}