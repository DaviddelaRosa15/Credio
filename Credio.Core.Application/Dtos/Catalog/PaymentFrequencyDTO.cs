using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Dtos.Catalog
{
    public class PaymentFrequencyDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int DaysInterval { get; set; }
    }
}
