using Credio.Core.Application.Helpers;
using Credio.Core.Application.Interfaces.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Credio.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            #region Configurations
            #endregion

            #region Services
            services.AddScoped<IEmailHelper, EmailHelper>();
            #endregion
        }
    }
}
