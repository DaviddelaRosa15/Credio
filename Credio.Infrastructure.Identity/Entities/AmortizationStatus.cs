using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class AmortizationStatus : BaseEntity
{
    public AmortizationStatus()
    {
        AmortizationSchedules = new HashSet<AmortizationSchedule>();
    }
    
    public ICollection<AmortizationSchedule> AmortizationSchedules { get; set; }
}