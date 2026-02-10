using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities;

public class Payment : AuditableBaseEntity
{
    public string LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public string EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public double AmountPaid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string PaymentMethodId { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = null!;

    public string PaymentStatusId { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = null!;

    public double? GpsLatitude { get; set; } 
    
    public double? GpsLongitude { get; set; } 

    public string? ReceiptUrl { get; set; } = string.Empty;
}