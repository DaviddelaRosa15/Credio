using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Seeds
{
    public static class DefaultAmortizationStatus
    {
        public static async Task SeedAsync(IAmortizationStatusRepository amortizationStatusRepository)
        {
            List<AmortizationStatus> amortizationStatusList = new();
            try
            {
                var amortizationStatuses = new List<string>
                    {
                        "Pendiente",
                        "Pagada",
                        "Atrasada"
                    };

                foreach (var item in amortizationStatuses)
                {
                    AmortizationStatus amortizationStatus = new()
                    {
                        Name = item,
                        Description = item
                    };

                    amortizationStatusList.Add(amortizationStatus);
                }

                await amortizationStatusRepository.AddManyAsync(amortizationStatusList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}