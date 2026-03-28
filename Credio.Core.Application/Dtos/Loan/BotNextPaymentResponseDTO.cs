namespace Credio.Core.Application.Dtos.Loan;

public class BotNextPaymentResponseDTO
{
    public BotPaymentDetailDTO? UrgentPayment { get; set; }

    public List<BotPaymentDetailDTO> OtherActiveLoans { get; set; }
}

public class BotPaymentDetailDTO
{
    public int LoanNumber { get; set; }

    public DateOnly DueTime { get; set; }

    public double InstallmentAmount { get; set; }

    public double LateFeeAmount { get; set; }

    public double TotalAmountToPay { get; set; }

    public int? DaysUntilDue { get; set; }
}