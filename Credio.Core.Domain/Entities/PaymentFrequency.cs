namespace Credio.Core.Domain.Entities;

public class PaymentFrequency
{
    public PaymentFrequency()
    {
        Id = Guid.NewGuid().ToString().Substring(0, 12);
        Loans = [];
    }
    
    public virtual string Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DaysInterval { get; set; }

    public bool IsActive { get; set; }
    
    public List<Loan> Loans { get; set; }
}