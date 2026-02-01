using Swashbuckle.AspNetCore.SwaggerUI;

namespace Credio.Interface.Authentication.Extensions
{
    public static class AppExtensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Credio Authentication API");
                options.DefaultModelRendering(ModelRendering.Model);
            });
        }
    }
}
