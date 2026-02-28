using Credio.Core.Application.Dtos.Common;

namespace Credio.Core.Application.Dtos.Client
{
    public class ClientDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string OfficerId { get; set; }

        public string Phone { get; set; }

        public string? Email { get; set; }

        public string? AddressId { get; set; }
    }
}

