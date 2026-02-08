using Credio.Core.Application;
using Credio.Infrastructure.Identity;
using Credio.Infrastructure.Persistence;
using Credio.Infrastructure.Shared;
using Credio.Interface.Authentication.Extensions;
using Credio.Authentication.Api.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    // Load environment variables
    DotNetEnv.Env.Load();
    
    // Configure logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
    
    // Add environment variables to configuration
    builder.Configuration.AddEnvironmentVariables();
    
    // Add services to the container.
    builder.Services
        .AddInterfaceLayer()
        .AddApplicationLayer()
        .AddPersistenceInfrastructure(builder.Configuration)
        .AddIdentityInfrastructure(builder.Configuration)
        .AddSharedInfrastructure(builder.Configuration);
}
// Build the application
WebApplication app = builder.Build();
{
    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler();
        app.UseHsts();
    }
    
    app.UseHttpsRedirection();
    app.UseCors("AllowSpecificDomain");
    app.UseMiddleware<FallBackRouteMiddleware>();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSwaggerExtension();
    app.UseHealthChecks("/health");
    app.UseSession();
    app.MapControllers();
    
    // Run the application
    app.Run();
}