using Credio.Core.Application.Constants;
using Credio.Core.Application.Helpers;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Settings;
using Credio.Infrastructure.Identity.Contexts;
using Credio.Infrastructure.Identity.Entities;
using Credio.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using Credio.Infrastructure.Identity.Workers;

namespace Credio.Infrastructure.Identity
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            
            #region Vaciar tablas
            /*var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseNpgsql(connection, m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
            var context = new IdentityContext(optionsBuilder.Options);
			context.TruncateTables();*/
            #endregion

            #region Contexts
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityContext>(options => options.UseInMemoryDatabase("BaseDb"));
            }
            else
            {
                var connection = configuration.GetConnectionString("DBCredioIdentity");
                var parameters = configuration["DBCREDIOIDENTITY"];
                connection = connection.Replace("%DBCREDIOIDENTITY%", parameters);

                services.AddDbContext<IdentityContext>(options =>
                {
                    options.EnableSensitiveDataLogging();
                    options.UseNpgsql(connection,
                    m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
                });
            }
            #endregion

            #region Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/User";
                options.AccessDeniedPath = "/User/AccessDenied";
            });

            // Access to replace placeholders
            var jwtSettingsSection = configuration.GetSection("JWTSettings");
            var jwtSettings = jwtSettingsSection.Get<JWTSettings>();
            var refreshSettingsSection = configuration.GetSection("RefreshJWTSettings");
            var refreshSettings = refreshSettingsSection.Get<RefreshJWTSettings>();

            // Replace placeholders in settings
            jwtSettings.Key = configuration["JWTKEY"];
            refreshSettings.Key = configuration["REFRESHKEY"];

            // Configuring JWT settings
            services.Configure<JWTSettings>(options =>
            {
                options.Key = jwtSettings.Key;
                options.Issuer = jwtSettings.Issuer;
                options.Audience = jwtSettings.Audience;
                options.DurationInMinutes = jwtSettings.DurationInMinutes;
            });

            // Configuring RefreshJWT settings
            services.Configure<RefreshJWTSettings>(options =>
            {
                options.Key = refreshSettings.Key;
                options.Issuer = refreshSettings.Issuer;
                options.Audience = refreshSettings.Audience;
                options.DurationInMinutes = refreshSettings.DurationInMinutes;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = c =>
                    {
                        c.HandleResponse();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ErrorMapperHelper.Error(ErrorMessages.Unauthorized, "Usuario no autorizado"));
                        return c.Response.WriteAsync(result);
                    },
                    OnForbidden = c =>
                    {
                        c.Response.StatusCode = 403;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ErrorMapperHelper.Error(ErrorMessages.Unauthorized, "Usted no está autorizado para usar este endpoint"));
                        return c.Response.WriteAsync(result);
                    }
                };

            });
            #endregion

            #region Services
            services.AddTransient<IAccountService, AccountService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            #endregion

            #region Workers
            services.AddHostedService<IdentitySeederWorker>();
            #endregion

            return services;
        }
    }
}
