namespace Credio.Core.Application.Dtos.LoanApplication;

public class BotApplicationStatusDTO
{
    public string ApplicationCode { get; set; }

    public double RequestedAmount { get; set; }
    
    public string StatusName { get; set; }

    public DateOnly? LastUpdateDate { get; set; }
}