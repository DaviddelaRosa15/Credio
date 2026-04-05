using Credio.Core.Domain.Settings;
using Credio.Lending.Api.ExceptionHandler;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Credio.Interface.Lending.Extensions;

public static class InterfaceExtensions
{
    public static IServiceCollection AddInterfaceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuración de EndOfDaySettings
        var endOfDayLogSettingsSection = configuration.GetSection("EndOfDayLogSettings");
        var eodSettings = endOfDayLogSettingsSection.Get<EndOfDayLogSettings>();

        // Validación de configuración
        services.Configure<EndOfDayLogSettings>(options =>
        {
            options.BatchSize = eodSettings.BatchSize;
            options.CheckFrequencyInMinutes = eodSettings.CheckFrequencyInMinutes;
        });

        // Core
        services
            .AddProblemDetails()
            .AddLogging()
            .AddMiddlewares()
            .AddEndpointsApiExplorer();
        
        services.AddControllers(options =>
            {
                options.Filters.Add(new ProducesAttribute("application/json"));
            })
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        
        /*
         *.ConfigureApiBehaviorOptions(options =>
           {
               options.SuppressInferBindingSourcesForParameters = true;
               options.SuppressMapClientErrors = false;
               options.SuppressModelStateInvalidFilter = false;
           })
         * 
         */

        services.AddHealthChecks();
        services.AddSwaggerExtension();
        services.AddApiVersioningExtension();
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Name = "MiSesion";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.AddSignalR();
        
        // Exception Handlers
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        // Cors
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificDomain",
                builder =>
                {
                    builder.WithOrigins("http://localhost:5173");
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                });
        });
        
        return services;
    }
}