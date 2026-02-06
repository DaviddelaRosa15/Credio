using Credio.Core.Application.Helpers;
using Credio.Core.Application.Interfaces.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Credio.Core.Application.Common.Pipelines;
using FluentValidation;

namespace Credio.Core.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // stops executing a validator class as soon as a rule fails
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);
            services.AddAutoMapper(typeof(ServiceRegistration).Assembly);
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly);
                
                // Pipelines Behaviors
                options.AddOpenBehavior(typeof(RequestValidationPipelineBehavior<,>));
                options.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
                options.AddOpenBehavior(typeof(RequestPerformancePipelineBehavior<,>));
                options.AddOpenBehavior(typeof(RequestExceptionHandlingPipelineBehavior<,>));
            });

            #region Services
            services.AddScoped<IEmailHelper, EmailHelper>();
            #endregion

            return services;
        }
    }
}
