using Credio.Core.Application.Dtos.Common;
using Credio.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Dtos.Employee
{
    public class EmployeeCollectorDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Phone { get; set; }

        public AddressDTO? Address { get; set; }
    }
}
