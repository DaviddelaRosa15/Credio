namespace Credio.Core.Application.Interfaces.Services
{
    public interface IReceiptNumberGeneratorService
    {
        Task<string> GenerateReceiptNumberAsync(string loanNumber, string loanId);
    }
}