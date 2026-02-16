using Credio.Infrastructure.Identity.Entities;
using Credio.Infrastructure.Identity.Seeds;
using Credio.Infrastructure.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Identity.Workers;

public class IdentitySeederWorker: BaseWorker<IdentitySeederWorker>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public IdentitySeederWorker(
        ILogger<BaseWorker<IdentitySeederWorker>> logger,
        IServiceScopeFactory scopeFactory) : base(logger)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // TODO: Uncomment when roles are ready
        // This will enable automatic seeding on startup.

        if (!roleManager.Roles.Any())
        {
            _logger.LogInformation("Roles doesn't exist. Seeding.");
            await DefaultRoles.SeedAsync(userManager, roleManager);
        }
        else
        {
            _logger.LogInformation("Roles already exist. Skipping seeding.");
        }

        if (!userManager.Users.Any())
        {
            _logger.LogInformation("Users doesn't exist. Seeding.");
            await DefaultSuperAdminUser.SeedAsync(userManager, roleManager);
        }
        else
        {
            _logger.LogInformation("Users already exist. Skipping seeding.");
        }
    }
}