using Messenger.API.Extensions;
using Messenger.API.HealthChecks;
using Messenger.API.Swagger;
using Messenger.Application.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Messenger.API;

public static class DependencyInjection
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<AppSettings>(
            builder.Configuration.GetSection("AppSettings"));


        builder.Services.AddJwtConfiguration(builder);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerConfiguration(builder);
        builder.Services.AddRateLimitingPolicies();

        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
    }

    private static void AddJwtConfiguration(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("AppSettings:JwtSettings").Get<JwtSettings>()
                    ?? throw new InvalidOperationException("Jwt settings are missing.");
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
            });
    }

}
