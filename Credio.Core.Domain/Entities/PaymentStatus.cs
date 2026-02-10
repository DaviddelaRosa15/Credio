using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class PaymentStatus : BaseEntity
{
    public PaymentStatus()
    {
        Payments = [];
    }
    
    public List<Payment> Payments { get; set; }
}