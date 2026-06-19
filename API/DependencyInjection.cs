using Messenger.API.HealthChecks;
using Messenger.API.Realtime;
using Messenger.API.Swagger;
using Messenger.Application.Abstractions.Realtime;
using Messenger.Application.Common.Models;
using Messenger.Infrastructure.Realtime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
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

        // Use a Redis backplane + shared presence store when configured so SignalR can fan out and
        // presence is consistent across multiple API instances; without it the hub works in
        // single-server mode with in-memory presence (fine for local dev).
        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            signalR.AddStackExchangeRedis(redisConnection, options =>
                options.Configuration.ChannelPrefix = RedisChannel.Literal("messenger"));

            builder.Services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(redisConnection));
            builder.Services.AddSingleton<IPresenceTracker, RedisPresenceTracker>();
        }
        else
        {
            builder.Services.AddSingleton<IPresenceTracker, InMemoryPresenceTracker>();
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
