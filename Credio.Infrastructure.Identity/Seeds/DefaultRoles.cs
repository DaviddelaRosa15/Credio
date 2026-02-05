using Credio.Core.Application.Enums;
using Credio.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Credio.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
	{
		public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Administrator.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Client.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Collector.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Officer.ToString()));
		}
	}
}
