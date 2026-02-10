using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class PaymentMethod : BaseEntity
{
    public PaymentMethod()
    {
        Payments = [];
    }
    
    public List<Payment> Payments { get; set; }
}