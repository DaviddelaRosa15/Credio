using Credio.Core.Application.Constants;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Seeds
{
    public static class DefaultPaymentMethod
    {
        public static async Task SeedAsync(IPaymentMethodRepository paymentMethodRepository)
        {
            List<PaymentMethod> paymentMethodList = new();
            try {
                var paymentMethod = new List<string>
                {
                    PaymentMethods.Efectivo,
                    PaymentMethods.Transferencia
                };

                foreach (var item in paymentMethod)
                {
                    PaymentMethod payment = new()
                    {
                        Name = item,
                        Description = item
                    };

                    paymentMethodList.Add(payment);
                }

                await paymentMethodRepository.AddManyAsync(paymentMethodList);


            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
