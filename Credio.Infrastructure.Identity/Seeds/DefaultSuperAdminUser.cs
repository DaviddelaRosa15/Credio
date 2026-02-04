using Credio.Core.Application.Enums;
using Credio.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Credio.Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser defaultUser = new();
            defaultUser.UserName = "superAdminUser";
            defaultUser.Email = "superadminuser@email.com";
            defaultUser.FirstName = "SuperUser";
            defaultUser.LastName = "SuperUser";
            defaultUser.EmailConfirmed = true;
            defaultUser.PhoneNumberConfirmed = true;

            await userManager.CreateAsync(defaultUser, "1505Pa@@word");
            await userManager.AddToRoleAsync(defaultUser, Roles.Analyst.ToString());
            await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
            await userManager.AddToRoleAsync(defaultUser, Roles.Administrator.ToString());
            await userManager.AddToRoleAsync(defaultUser, Roles.Guest.ToString());

        }
    }
}
