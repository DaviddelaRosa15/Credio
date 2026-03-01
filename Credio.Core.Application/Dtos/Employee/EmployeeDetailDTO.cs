using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Dtos.Employee
{
    public class EmployeeDetailDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string Phone { get; set; }

        public string? Email { get; set; }

        public string EmployeeCode { get; set; }

        public AddressDTO Address { get; set; }

        public List<ClientBasicDTO> Clients { get; set; }

    }
}
