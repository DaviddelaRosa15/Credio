using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Dtos.Payment
{
    public class PaymentHistoryDTO
    {
        public string PaymentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string PaymentMethodName { get; set; }

        public double AmountPaid { get; set; }


    }
}