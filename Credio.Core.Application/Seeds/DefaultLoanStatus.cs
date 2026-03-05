using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Seeds
{
    public static class DefaultLoanStatus
    {
        public static async Task SeedAsync(ILoanStatusRepository loanStatusRepository)
        {
            List<LoanStatus> loanStatusList = new();
            try
            {
                var loanStatuses = new List<string>
                    {
                        "Activo",
                        "Pagado",
                        "En mora"
                    };

                foreach (var item in loanStatuses)
                {
                    LoanStatus loanStatus = new()
                    {
                        Name = item,
                        Description = item,
                        IsActive = true
                    };

                    loanStatusList.Add(loanStatus);
                }

                await loanStatusRepository.AddManyAsync(loanStatusList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}