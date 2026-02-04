using Credio.Core.Domain.Common;

namespace Credio.Infrastructure.Identity.Entities;

public class Loan : AuditableBaseEntity
{
    public Loan()
    {
        LateFees = new HashSet<LateFee>();
        AmortizationSchedules = new HashSet<AmortizationSchedule>();
    }
    
    public int LoanNumber { get; set; }

    public string ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public string EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public string LoanApplicationId { get; set; }

    public LoanApplication LoanApplication { get; set; } = null!;

    public string LoanStatusId { get; set; }

    public LoanStatus LoanStatus { get; set; } = null!;

    public string PaymentFrequencyId { get; set; }

    public PaymentFrequency PaymentFrequency { get; set; } = null!;

    public string AmortizationMethodId { get; set; }

    public AmortizationMethod AmortizationMethod { get; set; } = null!;

    public double Amount { get; set; } 

    public double InterestRate { get; set; } 

    public int TermMonths { get; set; }

    public DateTime? DisbursedDate { get; set; }

    public DateTime FirstPaymentDate { get; set; }

    public ICollection<LateFee> LateFees { get; set; }

    public ICollection<AmortizationSchedule> AmortizationSchedules { get; set; }
}