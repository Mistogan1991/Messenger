using Messenger.API.HealthChecks;
using Messenger.API.Realtime;
using Messenger.API.Swagger;
using Messenger.Application.Abstractions.Realtime;
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

        var signalR = builder.Services.AddSignalR();

        // Use a Redis backplane when configured so SignalR can fan out across multiple API
        // instances; without it the hub works in single-server mode (fine for local dev).
        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            signalR.AddStackExchangeRedis(redisConnection, options =>
                options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal("messenger"));
        }

        builder.Services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();

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

                // WebSocket clients can't send an Authorization header, so accept the JWT from the
                // access_token query string for hub connections.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }

}
