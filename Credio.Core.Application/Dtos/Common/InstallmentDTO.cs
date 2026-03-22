namespace Credio.Core.Application.Dtos.Common
{
    public record InstallmentDTO(
        int InstallmentNumber,
        DateOnly DueDate,
        decimal DueAmount,
        decimal PrincipalAmount,
        decimal InterestAmount,
        decimal RemainingBalance
     );
}