namespace Credio.Core.Application.Dtos.LoanApplication;

public class LoanApplicationDto
{
    public string Id { get; set; }

    public string ApplicationCode { get; set; }

    public string ClientId { get; set; }

    public string ClientName { get; set; }
    
    public string EmployeeId { get; set; }

    public double RequestedAmount { get; set; }
    
    public int RequestTerm { get; set; }
    
    public double RequestedInterestRate { get; set; } 
    
    public string? Purpose { get; set; } 
    
    public double? ApprovedAmount { get; set; }  

    public int? ApprovedTerm { get; set; }
    
    public double? ApprovedInterestRate { get; set; }
    
    public string ApplicationStatusId { get; set; }

    public string ApplicationStatusName { get; set; }
    
    public string? RejectionReason { get; set; } = string.Empty;
}