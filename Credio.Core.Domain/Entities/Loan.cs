using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class Loan : AuditableBaseEntity
{
    public Loan()
    {
        LateFees = [];
        AmortizationSchedules = [];
        LoanBalances = [];
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

    public List<LateFee> LateFees { get; set; }

    public List<AmortizationSchedule> AmortizationSchedules { get; set; }

    public List<LoanBalance> LoanBalances { get; set; }
}