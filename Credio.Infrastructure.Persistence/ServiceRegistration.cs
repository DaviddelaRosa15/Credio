using Credio.Core.Application.Interfaces.Repositories;
using Credio.Infrastructure.Persistence.Contexts;
using Credio.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Credio.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region Vaciar tablas
            /*var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseNpgsql(connection, m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName));
            var context = new ApplicationContext(optionsBuilder.Options);
			context.TruncateTables();*/
            #endregion

            #region Contexts
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("BaseDb"));
            }
            else
            {
                var connection = configuration.GetConnectionString("PostgreSQL");
                var parameters = configuration["POSTGRESQL"];
                connection = connection.Replace("%POSTGRESQL%", parameters);

                services.AddDbContextFactory<ApplicationContext>(options =>
                {
                    options.EnableSensitiveDataLogging();
                    options.UseNpgsql(connection,
                    m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName));
                });
            }
            #endregion

            #region Repositories
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            #endregion
        }
    }
}
