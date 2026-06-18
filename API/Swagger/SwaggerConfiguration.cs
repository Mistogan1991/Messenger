using Microsoft.OpenApi;

namespace Messenger.API.Swagger;

public static class SwaggerConfiguration
{
    public static void AddSwaggerConfiguration(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        services.AddSwaggerGen(options =>
        {
            var securitySchemeId = "Bearer";
            options.AddSecurityDefinition(securitySchemeId, new OpenApiSecurityScheme
            {
                Description = @"Example (Just write token): 'AccessToken'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(securitySchemeId, document)] = []
            });
        });
    }

    public static void UseSwaggerConfiguration(this WebApplication app)
    {
        app.UseSwagger(c => { c.RouteTemplate = "api/swagger/{documentName}/swagger.json"; });
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "api/swagger";
            options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Messenger Api V1");
            options.DefaultModelsExpandDepth(10);
            options.EnablePersistAuthorization();
        });
    }
}
