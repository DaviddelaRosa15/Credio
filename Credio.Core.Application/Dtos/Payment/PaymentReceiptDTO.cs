using Credio.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Dtos.Payment
{
    public class PaymentReceiptDTO
    {
        public string Id { get; set; }
        public DateTime? PaymentDate { get; set; }

        public double AmountPaid { get; set; }

        public string ClientFullName { get; set; }

        public int LoanNumber { get; set; }

        public string PaymentMethodName { get; set; }

        public int PaidInstallments { get; set; }

        public int TotalInstallments { get; set; }

        public double PendingBalance { get; set; }
    }
}
