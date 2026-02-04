using Microsoft.AspNetCore.Identity;

namespace Credio.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
	{
		public ApplicationUser()
		{
			Employees = new HashSet<Employee>();
			Clients =  new HashSet<Client>();
		}
		
		public string FirstName { get; set; }
		
		public string LastName { get; set; }

		public bool? IsActive { get; set; }

        public ICollection<Employee> Employees { get; set; }

        public ICollection<Client> Clients { get; set; }
    }
}
