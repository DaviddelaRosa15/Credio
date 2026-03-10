using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Seeds
{
    public static class DefaultAmortizationMethod
    {
        public static async Task SeedAsync(IAmortizationMethodRepository amortizationMethodRepository)
        {
            List<AmortizationMethod> amortizationMethodList = new();
            try
            {
                var amortizationMethods = new List<string>
                    {
                        "Cuota Fija",
                        "Capital Fijo"
                    };

                foreach (var item in amortizationMethods)
                {
                    AmortizationMethod amortizationMethod = new()
                    {
                        Name = item,
                        Description = item
                    };

                    amortizationMethodList.Add(amortizationMethod);
                }

                await amortizationMethodRepository.AddManyAsync(amortizationMethodList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}