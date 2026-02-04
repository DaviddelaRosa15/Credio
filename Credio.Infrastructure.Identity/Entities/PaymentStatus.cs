using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class PaymentStatus : BaseEntity
{
    public PaymentStatus()
    {
        Payments = new HashSet<Payment>();
    }
    
    public ICollection<Payment> Payments { get; set; }
}