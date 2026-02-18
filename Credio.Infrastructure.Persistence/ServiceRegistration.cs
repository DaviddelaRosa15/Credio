using Credio.Infrastructure.Persistence.Contexts;
using Credio.Infrastructure.Persistence.Extensions;
using Credio.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Credio.Core.Application.Interfaces.Clients;
using Credio.Infrastructure.Persistence.Repositories;

namespace Credio.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddServices()
                .AddRepositories()
                .AddInterceptors()
                .AddWorkers();

            services.AddScoped<IClientQueryRepository, ClientQueryRepository>();    
            
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
                var connection = configuration.GetConnectionString("DBCredioCore");
                var parameters = configuration["DBCREDIOCORE"];
                connection = connection.Replace("%DBCREDIOCORE%", parameters);

                services.AddDbContextFactory<ApplicationContext>((provider, options) =>
                {
                    options.EnableSensitiveDataLogging();
                    options.UseNpgsql(connection,
                        builder => builder.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName))
                        .AddInterceptors(
                            provider.GetRequiredService<AuditableEntityInterceptor>(),
                            provider.GetRequiredService<SoftDeleteInterceptor>());
                });
            }
            #endregion

            return services;
        }
    }
}
