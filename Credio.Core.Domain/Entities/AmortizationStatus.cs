using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class AmortizationStatus : BaseEntity
{
    public AmortizationStatus()
    {
        AmortizationSchedules = [];
    }
    
    public List<AmortizationSchedule> AmortizationSchedules { get; set; }
}