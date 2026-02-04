namespace Credio.Infrastructure.Identity.Entities;

public class PaymentFrequency 
{
    public PaymentFrequency()
    {
        Id = Guid.NewGuid().ToString().Substring(0, 12);
        Loans = new HashSet<Loan>();
    }
    
    public virtual string Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DaysInterval { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Loan> Loans { get; set; }
}