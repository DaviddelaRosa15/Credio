using Credio.Core.Application;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Common;
using Credio.Infrastructure.Identity;
using Credio.Infrastructure.Identity.Entities;
using Credio.Infrastructure.Identity.Seeds;
using Credio.Infrastructure.Persistence;
using Credio.Infrastructure.Shared;
using Credio.Interface.Authentication.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
DotNetEnv.Env.Load();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Configure services
builder.Services.AddCors(options =>
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

builder.Services.AddLogging();
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressInferBindingSourcesForParameters = true;
    options.SuppressMapClientErrors = true;
    options.SuppressModelStateInvalidFilter = true;
})
.AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHealthChecks();
builder.Services.AddSwaggerExtension();
builder.Services.AddApiVersioningExtension();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "MiSesion";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSignalR();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowSpecificDomain");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerExtension();
app.UseHealthChecks("/health");
app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Middleware personalizado para rutas no definidas
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        ErrorDTO errorResponse = new()
        {
            Status = "Fallido",
            Details = [new ErrorDetailsDTO() { Code = ErrorMessages.NotFound, Message = "La ruta solicitada no existe en la API." }]
        };
        var json = System.Text.Json.JsonSerializer.Serialize(errorResponse);

        context.Response.ContentLength = null; // Previene conflictos de longitud
        await context.Response.WriteAsync(json);
    }
});

// Seed data
if (builder.Configuration.GetValue<bool>("InitialRun"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            #region Identity
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await DefaultRoles.SeedAsync(userManager, roleManager);
            await DefaultSuperAdminUser.SeedAsync(userManager, roleManager);
            #endregion
        }
        catch (Exception ex)
        {
            // Handle exceptions
        }
    }
}

// Run the application
app.Run();
