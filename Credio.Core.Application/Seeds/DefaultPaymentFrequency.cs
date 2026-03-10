using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Seeds
{
    public static class DefaultPaymentFrequency
    {
        public static async Task SeedAsync(IPaymentFrequencyRepository paymentFrequencyRepository)
        {            
            try
            {
                List<PaymentFrequency> paymentFrequencyList =
                [
                    new()
                    {
                        Name = "Mensual",
                        DaysInterval = 30,
                        IsActive = true
                    },
                    new()
                    {
                        Name = "Quincenal",
                        DaysInterval = 15,
                        IsActive = true
                    },
                    new()
                    {
                        Name = "Semanal",
                        DaysInterval = 7,
                        IsActive = true
                    },
                ];

                await paymentFrequencyRepository.AddManyAsync(paymentFrequencyList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}