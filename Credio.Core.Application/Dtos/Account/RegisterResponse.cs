using Credio.Core.Application.Dtos.Common;

namespace Credio.Core.Application.Dtos.Account
{
    public class RegisterResponse : ErrorDTO
	{
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string UrlImage { get; set; }
    }
}
