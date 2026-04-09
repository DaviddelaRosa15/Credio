using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Core.Application.Constants;

namespace Credio.Core.Application.Seeds
{
    public static class DefaultPaymentStatus
    {
        public static async Task SeedAsync(IPaymentStatusRepository paymentStatusRepository)
        {
            List<PaymentStatus> list = new();
            try
            {
                var paymentStatus = new List<string>
                    {
                        PaymentStatuses.Completado,
                        PaymentStatuses.Pendiente,
                        PaymentStatuses.Anulado
                    };

                foreach (var item in paymentStatus)
                {
                    PaymentStatus status = new()
                    {
                        Name = item,
                        Description = item
                    };

                    list.Add(status);
                }

                await paymentStatusRepository.AddManyAsync(list);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
