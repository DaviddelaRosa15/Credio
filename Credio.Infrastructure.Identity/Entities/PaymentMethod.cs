using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class PaymentMethod : BaseEntity
{
    public PaymentMethod()
    {
        Payments = new HashSet<Payment>();
    }
    
    public ICollection<Payment> Payments { get; set; }
}