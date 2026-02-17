using Credio.Core.Application.Dtos.Common;

namespace Credio.Core.Application.Dtos.Employee
{
    public class EmployeeDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string Phone { get; set; }

        public string? Email { get; set; }

        public string EmployeeCode { get; set; }

        public string? AddressId { get; set; }

        public AddressDTO Address { get; set; }
    }
}
